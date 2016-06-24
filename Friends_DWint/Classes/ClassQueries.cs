using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Web;


namespace Friends_DWint
{
    class ClassQueries 
    {
        public string loadPage(string url)
        {

            WebResponse resp = null;
            StreamReader reader = null;
            string htmlCode = "";
            try
            {
                Uri uri = new Uri(url);
                try
                {
                    resp = WebRequest.Create(url).GetResponse();
                    reader = new StreamReader(resp.GetResponseStream());
                    htmlCode = reader.ReadToEnd();
                    htmlCode = HttpUtility.HtmlDecode(htmlCode);
                }
                //catch (WebException ex) { MessageBox.Show(ex.Message); }
                //catch (ProtocolViolationException ex) { MessageBox.Show(ex.Message); }
                //catch (UriFormatException ex) { MessageBox.Show(ex.Message); }
                //catch (IOException ex) { MessageBox.Show(ex.Message); }
                //catch (NotSupportedException ex) { MessageBox.Show(ex.Message); }
                catch (System.Security.SecurityException ex) { MessageBox.Show(ex.Message); }
                catch { }
                finally
                {
                    if (resp != null)
                        resp.Close();
                    if (reader != null)
                        reader.Close();

                }
            }
          //  catch (Exception e) { MessageBox.Show(e.Message); }
            catch {}
            return htmlCode;
        }

    }
}
