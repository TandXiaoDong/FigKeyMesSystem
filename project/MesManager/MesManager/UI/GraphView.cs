using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.Charting;
using ZedGraph;

namespace MesManager.UI
{
    public partial class GraphView : RadForm
    {
        public GraphView()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void GraphView_Load(object sender, EventArgs e)
        {
            Draw_Load();
        }
        private void Draw_Load()
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();
            GraphPane myPane = zedGraphControl1.GraphPane;
            // 画图面版X标题  
            myPane.XAxis.Title.Text = "区域";
            // myPane.XAxis.Type = AxisType.Text;
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 20;
            //myPane.YAxis.Scale.MinorStep = 10;
            //myPane.YAxis.Scale.MajorStep = 10;
            myPane.Chart.Border.IsVisible = false;
            myPane.YAxis.MajorTic.IsOpposite = false;
            //初始化数据  
            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            for (int i = 0; i < 100; i++)////这里的数量要和lable的一致，比如横坐标显示了5个lable，这里就要给5个  
            {
                list.Add(i, i + 10);
                list2.Add(i, i + 20);
            }
            // 画图面版Y标题  
            myPane.YAxis.Title.Text = "销售情况";
            //柱的画笔  
            BarItem myCurve = myPane.AddBar("收入1", list, Color.Blue);
            BarItem barItem = myPane.AddBar("收入2",list2,Color.Red);
            // BarItem myCurve = myPane.AddBar("收入1", null, Color.Blue);
            //BarItem myCurve1 = myPane.AddBar("收入2", list2, Color.Green);
            for (int i = 0; i < myCurve.Points.Count; i++)
            {
                //PointPair pt = myCurve.Points[i];
                //TextObj text = new TextObj(pt.Y.ToString("f2"), float.Parse(pt.X + ""), float.Parse(pt.Y + offset + ""), CoordType.AxisXYScale, AlignH.Right, AlignV.Center);
                //text.ZOrder = ZOrder.A_InFront;

                //// 隐藏标注的边框和填充
                //text.FontSpec.Border.IsVisible = false;
                //text.FontSpec.Fill.IsVisible = false;
                //// 选择标注字体90°
                ////text.FontSpec.Angle = 90;

                //myPane.GraphObjList.Add(text);

                //pt = myCurve1.Points[i];
                //text = new TextObj(pt.Y.ToString("f2"), float.Parse(pt.X + ""), float.Parse(pt.Y + offset + ""), CoordType.AxisXYScale, AlignH.Left, AlignV.Center);
                //text.FontSpec.Border.IsVisible = false;
                //text.FontSpec.Fill.IsVisible = false;
                //myPane.GraphObjList.Add(text);
            }

            //myPane.XAxis.MajorTic.IsBetweenLabels = true;  
            //XAxis标注  
            // string[] labels = { "产品1", "产品2", "产品3", "产品4", "产品5", "产品5", "产品5", "产品1", "产品2", "产品3", "产品4", "产品5", "产品5", "产品5" };
            //  string[] labels = { "产品1" };
            //  myPane.XAxis.Scale.TextLabels = labels;
            //  //myPane.XAxis.Scale.Min = 0;
            //  myPane.XAxis.Scale.Max = 10;
            ////  myPane.XAxis.Scale.MajorStep = 50;
            // /// myPane.XAxis.Scale.MaxAuto = true;
            //  myPane.XAxis.Type = AxisType.Text;
            //图区以外的颜色  fo
            // myPane.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);  
            //背景颜色  
            //myPane.Chart.Fill = new Fill(Color.Red, Color.LightGoldenrodYellow, 45.0f);  
            //myPane.Fill = new Fill(Color.White, Color.FromArgb(200, 200, 255), 45.0f);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();


            //DataTable dt = new DataTable("cart");
            //DataColumn dc1 = new DataColumn("areaid", Type.GetType("System.String"));
            //DataColumn dc2 = new DataColumn("house", Type.GetType("System.String"));
            //DataColumn dc3 = new DataColumn("seq", Type.GetType("System.String"));
            //DataColumn dc4 = new DataColumn("remark", Type.GetType("System.String"));

            //dt.Columns.Add(dc1);
            //dt.Columns.Add(dc2);
            //dt.Columns.Add(dc3);
            //dt.Columns.Add(dc4);


            //DataRow dr = dt.NewRow();
            //dr["areaid"] = "北京";
            //dr["house"] = "北京仓库";
            //dr["seq"] = "2";
            //dr["remark"] = "货到付款";
            //dt.Rows.Add(dr);


            //DataRow dr1 = dt.NewRow();
            //dr1["areaid"] = "北京";
            //dr1["house"] = "上海仓库";
            //dr1["seq"] = "1";
            //dr1["remark"] = "货到付款";
            //dt.Rows.Add(dr1);

            //DataRow dr2 = dt.NewRow();
            //dr2["areaid"] = "上海";
            //dr2["house"] = "上海仓库";
            //dr2["seq"] = "1";
            //dr2["remark"] = "货到付款";
            //dt.Rows.Add(dr2);

            //DataRow dr3 = dt.NewRow();
            //dr3["areaid"] = "上海";
            //dr3["house"] = "北京仓库";
            //dr3["seq"] = "1";
            //dr3["remark"] = "货到付款";
            //dt.Rows.Add(dr3);


            //var query = from cus in dt.AsEnumerable()
            //            group cus by new { t1 = cus.Field<string>("areaid"), t2 = cus.Field<string>("seq") } into m
            //            select new
            //            {
            //                areaid = m.Key.t1,
            //                seq = m.Key.t2,
            //                house = m.First().Field<string>("house"),
            //                rowcount = m.Count()
            //            };


            //Console.WriteLine("区域 " + "  库房" + "   数量");
            //foreach (var item in query.ToList())
            //{
            //    if (item.rowcount > 1)
            //    {
            //        MessageBox.Show(item.areaid + "---" + item.house);
            //    }
            //    Console.WriteLine(item.areaid + "---" + item.house + "---" + item.rowcount);
            //    Console.WriteLine("\r\n");
            //}

        }
    }
}
