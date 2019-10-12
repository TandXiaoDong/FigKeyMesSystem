using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lassalle;
using Lassalle.DlgFlow;
using Lassalle.Flow;
using Lassalle.GraphAlgo;
using Lassalle.PrnFlow;
using Lassalle.XMLFlow;
using GDIDrawFlow;
using WeifenLuo.WinFormsUI.Docking;

namespace TestAPI
{
    public partial class Form2 : DockContent
    {
        private AddFlow addFlow1;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            addFlow1 = new AddFlow();
            this.addFlow1.BackColor = SystemColors.Window;
            this.addFlow1.CursorSetting = Lassalle.Flow.CursorSetting.All;
            //            this.addFlow1.MouseAction = MouseAction.
            this.addFlow1.DefNodeProp.Shape.Style = ShapeStyle.RectEdgeRaised;
            this.addFlow1.DefNodeProp.FillColor = SystemColors.Control;
            this.addFlow1.DefLinkProp.Jump = Jump.Arc;
            //this.addFlow1.DefLinkProp.MaxPointsCount = 3;

            GDIDrawFlow.DrawFlowGroup drawFlowGroup1 = new DrawFlowGroup();
            drawFlowGroup1.Dock = System.Windows.Forms.DockStyle.Fill;
            drawFlowGroup1.Location = new System.Drawing.Point(0, 0);
            drawFlowGroup1.Name = "drawFlowGroup1";
            drawFlowGroup1.Size = new System.Drawing.Size(704, 502);
            drawFlowGroup1.TabIndex = 0;

            //this.Controls.Add(drawFlowGroup1);
        }



        private void btnNode_Click(object sender, System.EventArgs e)
        {
            //this.addFlow1.CanStretchLink = false;
            this.addFlow1.CanDrawLink = false;
            this.addFlow1.CanDrawNode = true;

        }

        private void addFlow1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //this.addFlow1.Nodes.Remove()
            foreach (Lassalle.Flow.Item item in this.addFlow1.SelectedItems)
            {
                //this.addFlow1.Nodes.Remove(item);
                //this.addFlow1.SelectedItems.RemoveAt(0);
            }

        }

        private void btnLink_Click(object sender, System.EventArgs e)
        {
            this.addFlow1.CanDrawLink = true;
            this.addFlow1.CanDrawNode = false;
        }

        private void btnSelete_Click(object sender, System.EventArgs e)
        {
            this.addFlow1.CanDrawLink = false;
            this.addFlow1.CanDrawNode = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Lassalle.Flow.Node n1 = this.addFlow1.Nodes.Add(10, 10, 40, 40, "aaa");
            Lassalle.Flow.Node n2 = this.addFlow1.Nodes.Add(60, 60, 40, 40, "bbb");

            //Lassalle.Flow.Link l = new Lassalle.Flow.Link()

            Lassalle.Flow.Link l = n1.Links.Add(n2, "lll");
            //            l.MaxPointsCount = 3;

            //n1.Shape = Lassalle.Flow.Shape.
        }
    }
}
