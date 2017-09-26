using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KdTreeDemo
{
    public class KDTree
    {
        public KDTreeNode rootNode;

        private Stack<KDTreeNode> backtrackStack = new Stack<KDTreeNode>();

        public void CreateByPointList(List<Point> pointList)
        {
            rootNode = CreateTreeNode(pointList);
        }

        private KDTreeNode CreateTreeNode(List<Point> pointList)
        {
            if (pointList.Count > 0)
            {
                // 计算方差
                double xObtainVariance = ObtainVariance(CreateXList(pointList));
                double yObtainVariance = ObtainVariance(CreateYList(pointList));

                // 根据方差确定分裂维度
                EnumDivisionType divisionType = SortListByXOrYVariances(xObtainVariance, yObtainVariance, ref pointList);

                // 获得中位数
                Point medianPoint = ObtainMedian(pointList);
                int medianIndex = pointList.Count / 2;

                // 构建节点
                KDTreeNode treeNode = new KDTreeNode()
                {
                    DivisionPoint = medianPoint,
                    DivisionType = divisionType,
                    LeftChild = CreateTreeNode(pointList.Take(medianIndex).ToList()),
                    RightChild = CreateTreeNode(pointList.Skip(medianIndex + 1).ToList())
                };
                return treeNode;
            }
            else
            {
                return null;
            }
        }

        private double ObtainVariance(List<Double> numbers)
        {
            double average = numbers.Average();
            double sumValue = 0.0;
            numbers.ForEach(number =>
            {
                sumValue += Math.Pow((number - average), 2);
            });
            return sumValue / (double)numbers.Count;
        }

        private List<double> CreateXList(List<Point> pointList)
        {
            List<double> list = new List<double>();
            pointList.ForEach(item => list.Add(item.X));
            return list;
        }

        private List<double> CreateYList(List<Point> pointList)
        {
            List<double> list = new List<double>();
            pointList.ForEach(item => list.Add(item.Y));
            return list;
        }

        private EnumDivisionType SortListByXOrYVariances(double xVariance, double yVariance, ref List<Point> pointList)
        {
            if (xVariance > yVariance)
            {
                pointList.Sort((point1, point2) => point1.X.CompareTo(point2.X));
                return EnumDivisionType.X;
            }
            else
            {
                pointList.Sort((point1, point2) => point1.Y.CompareTo(point2.Y));
                return EnumDivisionType.Y;
            }
        }

        private Point ObtainMedian(List<Point> pointList)
        {
            return pointList[pointList.Count / 2];
        }

        public Point FindNearest(Point searchPoint)
        {
            Point nearestPoint = DFSSearch(this.rootNode, searchPoint);
            return BacktrcakSearch(searchPoint, nearestPoint);
        }

        private Point DFSSearch(KDTreeNode node, Point searchPoint, bool pushStack = true)
        {
            if (pushStack == true)
            {
                backtrackStack.Push(node);
            }
            if (node.DivisionType == EnumDivisionType.X)
            {
                return DFSXsearch(node, searchPoint);
            }
            else
            {
                return DFSYsearch(node, searchPoint);
            }
        }

        private Point DFSBackTrackingSearch(KDTreeNode node, Point searchPoint)
        {
            backtrackStack.Push(node);

            if (node.DivisionType == EnumDivisionType.X)
            {
                return DFSBackTrackingXsearch(node, searchPoint);
            }
            else
            {
                return DFSBackTrackingYsearch(node, searchPoint);
            }
        }

        private Point DFSBackTrackingXsearch(KDTreeNode node, Point searchPoint)
        {
            if (node.DivisionPoint.X > searchPoint.X)
            {
                node.LeftChild = null;
                Point rightSearchPoint = DFSBackTrackRightSearch(node, searchPoint);
                node.RightChild = null;
                return rightSearchPoint;
            }
            else
            {
                node.RightChild = null;
                Point leftSearchPoint = DFSBackTrackLeftSearch(node, searchPoint);
                node.LeftChild = null;
                return leftSearchPoint;
            }
        }

        private Point DFSBackTrackLeftSearch(KDTreeNode node, Point searchPoint)
        {
            if (node.LeftChild != null)
            {
                return DFSSearch(node.LeftChild, searchPoint, false);
            }
            else
            {
                return node.DivisionPoint;
            }
        }

        private Point DFSBackTrackRightSearch(KDTreeNode node, Point searchPoint)
        {
            if (node.RightChild != null)
            {
                return DFSSearch(node.RightChild, searchPoint, false);
            }
            else
            {
                return node.DivisionPoint;
            }
        }

        private Point DFSBackTrackingYsearch(KDTreeNode node, Point searchPoint)
        {
            if (node.DivisionPoint.Y > searchPoint.Y)
            {
                node.LeftChild = null;
                Point rightSearchPoint = DFSBackTrackRightSearch(node, searchPoint);
                node.RightChild = null;
                return rightSearchPoint;
            }
            else
            {
                node.RightChild = null;
                Point leftSearchPoint = DFSBackTrackLeftSearch(node, searchPoint);
                node.LeftChild = null;
                return leftSearchPoint;
            }
        }

        private Point DFSXsearch(KDTreeNode node, Point searchPoint)
        {
            if (node.DivisionPoint.X > searchPoint.X)
            {
                return DFSLeftSearch(node, searchPoint);
            }
            else
            {
                return DFSRightSearch(node, searchPoint);
            }
        }

        private Point DFSYsearch(KDTreeNode node, Point searchPoint)
        {
            if (node.DivisionPoint.Y > searchPoint.Y)
            {
                return DFSLeftSearch(node, searchPoint);
            }
            else
            {
                return DFSRightSearch(node, searchPoint);
            }
        }

        private Point DFSLeftSearch(KDTreeNode node, Point searchPoint)
        {
            if (node.LeftChild != null)
            {
                return DFSSearch(node.LeftChild, searchPoint);
            }
            else
            {
                return node.DivisionPoint;
            }
        }

        private Point DFSRightSearch(KDTreeNode node, Point searchPoint)
        {
            if (node.RightChild != null)
            {
                return DFSSearch(node.RightChild, searchPoint);
            }
            else
            {
                return node.DivisionPoint;
            }
        }

        private Point BacktrcakSearch(Point searchPoint, Point nearestPoint)
        {
            if (backtrackStack.IsEmpty())
            {
                return nearestPoint;
            }
            else
            {
                KDTreeNode trackNode = backtrackStack.Pop();
                double backtrackDistance = ObtainDistanFromTwoPoint(searchPoint, trackNode.DivisionPoint);
                double nearestPointDistance = ObtainDistanFromTwoPoint(searchPoint, nearestPoint);
                if (backtrackDistance < nearestPointDistance)
                {
                    KDTreeNode searchNode = new KDTreeNode()
                    {
                        DivisionPoint = trackNode.DivisionPoint,
                        DivisionType = trackNode.DivisionType,
                        LeftChild = trackNode.LeftChild,
                        RightChild = trackNode.RightChild
                    };
                    nearestPoint = DFSBackTrackingSearch(searchNode, searchPoint);
                }
                return BacktrcakSearch(searchPoint, nearestPoint);
            }
        }

        private double ObtainDistanFromTwoPoint(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));
        }
    }
}
