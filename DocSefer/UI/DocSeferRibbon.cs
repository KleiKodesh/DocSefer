using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;

namespace DocSefer.UI
{
    [ComVisible(true)]
    public class DocSeferRibbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public DocSeferRibbon()
        {
        }

        public void button_Click(Office.IRibbonControl control)
        {
            Helpers.WpfTaskPane.Create(new DocSeferLib.DocSeferLibView(Globals.ThisAddIn.Application), "DocSefer", 500, true);

            //Stopwatch sw = Stopwatch.StartNew();

            //sw.Restart();
            //Helpers.WpfTaskPane.Create(new UserControl(), "DocSefer", 500);
            //Debug.WriteLine("UserControl load time: " + sw.ElapsedMilliseconds + " ms");

            //sw.Restart();
            //Helpers.WpfTaskPane.Create(new DocSeferView(), "DocSefer", 500);
            //Debug.WriteLine("DocSeferView (first) load time: " + sw.ElapsedMilliseconds + " ms");

            //sw.Restart();
            //Helpers.WpfTaskPane.Create(new DocSeferView2(), "DocSefer", 500);
            //Debug.WriteLine("DocSeferView2 (second) load time: " + sw.ElapsedMilliseconds + " ms");
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("DocSefer.UI.DocSeferRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
