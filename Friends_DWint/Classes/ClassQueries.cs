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
        public string loadPagePOST(string url, string _data, int count) {
            string htmlCode = "";
             try
             {
                 var request = (HttpWebRequest)WebRequest.Create(url);
                 var data = Encoding.ASCII.GetBytes(_data);
                 request.Timeout = 10000;
                 request.Method = "POST";
                 request.ContentType = "application/x-www-form-urlencoded";
                 request.ContentLength = data.Length;

                 using (var stream = request.GetRequestStream())
                 {
                     stream.Write(data, 0, data.Length);
                 }

                 var response = (HttpWebResponse)request.GetResponse();

                 var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                 htmlCode = HttpUtility.HtmlDecode(responseString);
             }catch (System.Security.SecurityException ex) { MessageBox.Show(ex.Message); }
                catch { }
            return htmlCode;
        }

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
                    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                    resp = wr.GetResponse();
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
