using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using QuickGraph;

namespace CoursePlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Microsoft.Msagl.Drawing.Graph g = new Microsoft.Msagl.Drawing.Graph("graph");
        static public System.Windows.Forms.Form form = new System.Windows.Forms.Form();
        static Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        static List<Graph> result = new List<Graph>();

        public static System.Windows.Forms.Form GetForm()
        {
            return form;
        }
        public static Microsoft.Msagl.GraphViewerGdi.GViewer GetViewer()
        {
            return viewer;
        }

        public static void changeColorToGreen(string node)
        {
            g.FindNode(node).Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightGreen;
            Task.Delay(1000).Wait();
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            form.Refresh();

        }
        public static void changeColorToRed(string node)
        {
            g.FindNode(node).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
            Task.Delay(1000).Wait();
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            form.Refresh();
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        static Graph convertFile(string filename) // Convert file into a graph
        {
            List<List<string>> linesList = new List<List<string>>();
            string line;
            char[] coma = new char[] { ',', ' ' };

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                List<string> lines = new List<string>();
                lines.AddRange(line.Split(coma, StringSplitOptions.RemoveEmptyEntries));
                lines[lines.Count - 1] = (lines[lines.Count - 1].Split('.'))[0];
                linesList.Add(lines);
            }
            file.Close();

            Graph graph = new Graph();
            for (int i = 0; i < linesList.Count; i++)
            {
                Node n = new Node();
                n.setCourse(linesList[i][0]);
                for (int j = 0; j < linesList.Count; j++)
                {
                    for (int k = 1; k < linesList[j].Count; k++)
                    {
                        if (n.getCourse() == linesList[j][k])
                        {
                            n.addReq(linesList[j][0]);
                        }
                    }
                }
                graph.addNode(n);
            }
            return graph;
        }
        static string printFinal(List<Graph> result) // Print the final result
        {
            string text = "";
            int i = 1;
            foreach (Graph g in result)
            {
                text += "Semester " + i + " : ";
                foreach (Node n in g.getNodes())
                {
                    text += n.getCourse() + " ";
                }
                text += "\n";
                i++;
            }
            return text;
        }
        
        private void BFS_Click(object sender, RoutedEventArgs e)
        {
            Graph graph = convertFile(filename.Text);
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            for (int i = 0; i < graph.getLenNodes(); i++)
            {
                for (int j = 0; j < graph.getNodes()[i].getLenReq(); j++)
                {
                    string a1 = graph.getNodes()[i].getCourse();
                    string a2 = graph.getNodes()[i].getReqN(j);
                    g.AddEdge(a1, a2).Attr.Color = Microsoft.Msagl.Drawing.Color.Pink;
                }
            }
            viewer.Graph = g;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.Show();

            Graph BFSgraph = new Graph();
            graph.BFS(BFSgraph);
            result = BFSgraph.seperateTerm();
            FinalAnswers.Text = printFinal(result);
            form.Refresh();
        }

        private void DFS_Click(object sender, RoutedEventArgs e)
        {
            Graph graph = convertFile(filename.Text);
            Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
            //create a graph object 
            //create the graph content 
            for (int i = 0; i < graph.getLenNodes(); i++)
            {
                for (int j = 0; j < graph.getNodes()[i].getLenReq(); j++)
                {
                    string a1 = graph.getNodes()[i].getCourse();
                    string a2 = graph.getNodes()[i].getReqN(j);
                    g.AddEdge(a1, a2).Attr.Color = Microsoft.Msagl.Drawing.Color.DeepSkyBlue;
                }
            }
            viewer.Graph = g;
            //associate the viewer with the form 
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form 
            form.Show();

            Graph DFSgraph = new Graph();
            Graph startNodes = graph.noPreReq();
            foreach (Node n in startNodes.getNodes())
            {
                graph.DFS(n, DFSgraph);
            }
            for (int i = 0; i < DFSgraph.getLenNodes(); i++)
            {
                Node current = DFSgraph.getNodes()[i];
                int max = current.getTerm();
                for (int j = 0; j < i; j++)
                {
                    Node check = DFSgraph.getNodes()[j];
                    foreach (string v in check.getReq())
                    {
                        if (current.getCourse() == v)
                        {
                            if (max < check.getTerm())
                            {
                                max = check.getTerm();
                            }
                        }
                    }
                }
                DFSgraph.setTermGraph(max + 1, i);
            }
            Console.WriteLine();
            result = DFSgraph.seperateTerm();
            FinalAnswers.Text = printFinal(result);
            form.Refresh();
        }

        private void FinalAnswers_TextChanged(object sender, TextChangedEventArgs e)
        {
            FinalAnswers.Text = printFinal(result);
        }
    }
}