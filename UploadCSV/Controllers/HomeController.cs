using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UploadCSV.Models;

namespace UploadCSV.Controllers
{
    public class HomeController : Controller
    {
        private IHostingEnvironment Environment;
        public HomeController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile postedFile)
        {
            if (postedFile != null)
            {
                string path = Path.Combine(this.Environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                string csvData = System.IO.File.ReadAllText(filePath);
                //string conString = ConfigurationManager.ConnectionStrings["Constring"].ConnectionString;
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[6] { new DataColumn("IzbornaJedinica", typeof(string)),
                                new DataColumn("DT", typeof(long)),
                                new DataColumn("HC", typeof(long)),
                                new DataColumn("JB", typeof(long)),
                                new DataColumn("JFK", typeof(long)),
                                new DataColumn("JR", typeof(long))

                });
                
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        if (!string.IsNullOrEmpty(row))
                        {

                            dt.Rows.Add();
                            int i = 0;
                            string[] cells = row.Split(',');

                            for (int j = 0; j < cells.Length; j++)
                            {
                                try
                                {
                                    string nextColumn = cells[j + 1].Trim();
                                    var isIzbornaJEdinicaValue = cells[j].Trim().Contains("New York")
                                                                 || cells[j].Trim().Contains("Washington")
                                                                 || cells[j].Trim().Contains("Texas");


                                    if (nextColumn == "DT")
                                        dt.Rows[dt.Rows.Count - 1]["DT"] = cells[j].Trim();
                                    else if (nextColumn == "HC")
                                        dt.Rows[dt.Rows.Count - 1]["HC"] = cells[j].Trim();
                                    else if (nextColumn == "JB")
                                        dt.Rows[dt.Rows.Count - 1]["JB"] = cells[j].Trim();
                                    else if (nextColumn == "JFK")
                                        dt.Rows[dt.Rows.Count - 1]["JFK"] = cells[j].Trim();
                                    else if (nextColumn == "JR")
                                        dt.Rows[dt.Rows.Count - 1]["JR"] = cells[j].Trim();
                                    else if (isIzbornaJEdinicaValue)
                                        dt.Rows[dt.Rows.Count - 1]["IzbornaJedinica"] = cells[j];
                                }
                                catch (Exception ex)
                                {

                                    string kandidatNotPopulated = string.Empty;
                                    if (dt.Rows[dt.Rows.Count - 1]["JR"].ToString() == "")
                                        kandidatNotPopulated = "JR";
                                    else if (dt.Rows[dt.Rows.Count - 1]["JFK"].ToString() == "")
                                        kandidatNotPopulated = "JFK";
                                    else if (dt.Rows[dt.Rows.Count - 1]["JB"].ToString() == "")
                                        kandidatNotPopulated = "JB";
                                    else if (dt.Rows[dt.Rows.Count - 1]["HC"].ToString() == "")
                                        kandidatNotPopulated = "HC";
                                    else if (dt.Rows[dt.Rows.Count - 1]["DT"].ToString() == "")
                                        kandidatNotPopulated = "DT";

                                    


                                }

                            }
                        }
                    }

                    
                }
                

                IList<IzboriVM.Rezultati> rezultati = dt.AsEnumerable().Select(row =>
    new IzboriVM.Rezultati
    {
        IzbornaJedinica = row.Field<string>("IzbornaJedinica"),
        DT = row.Field<long?>("DT"),
        HC = row.Field<long?>("HC"),
        JB = row.Field<long?>("JB"),
        JFK = row.Field<long?>("JFK"),
        JR = row.Field<long?>("JR")
    }).ToList();

                List<IzboriVM.RezultatiView> rezultatiview = new List<IzboriVM.RezultatiView>();
                foreach (var item in rezultati)
                {
                    decimal ukupniGlasovi = 0;
                    ukupniGlasovi += item.DT != null ?  Convert.ToDecimal(item.DT): 0;
                    ukupniGlasovi += item.HC != null ? Convert.ToDecimal(item.HC ): 0;
                    ukupniGlasovi += item.JB != null ? Convert.ToDecimal(item.JB) : 0;
                    ukupniGlasovi += item.JFK != null ?Convert.ToDecimal( item.JFK) : 0;
                    ukupniGlasovi += item.JR != null ? Convert.ToDecimal(item.JR) : 0;
                    IzboriVM.RezultatiView donald = new IzboriVM.RezultatiView();
                    donald.IzbornaJedinica = item.IzbornaJedinica;
                    donald.Kandidat = "Donald Trump";
                    donald.BrojGlasova = item.DT != null ? item.DT.ToString() : "N/A";
                    donald.Procenat = item.DT != null ? ((Convert.ToDecimal(item.DT) / ukupniGlasovi) * 100).ToString("0.00") + " %": "N/A";
                    donald.Error = item.DT != null ? "" : "Yes";
                    rezultatiview.Add(donald);

                    IzboriVM.RezultatiView hc = new IzboriVM.RezultatiView();
                    hc.IzbornaJedinica = item.IzbornaJedinica;
                    hc.Kandidat = "Hillary Clinton";
                    hc.BrojGlasova = item.HC != null ? item.HC.ToString() : "N/A";
                    hc.Procenat = item.HC != null ? ((Convert.ToDecimal(item.HC) / ukupniGlasovi) * 100).ToString("0.00") + " %" : "N/A";
                    hc.Error = item.HC != null ? "" : "Yes";
                    rezultatiview.Add(hc);

                    IzboriVM.RezultatiView jb = new IzboriVM.RezultatiView();
                    jb.IzbornaJedinica = item.IzbornaJedinica;
                    jb.Kandidat = "Joe Biden";
                    jb.BrojGlasova = item.JB != null ? item.JB.ToString() : "N/A";
                    jb.Procenat = item.JB != null ? ((Convert.ToDecimal(item.JB) / ukupniGlasovi) * 100).ToString("0.00") + " %" : "N/A";
                    jb.Error = item.JB != null ? "" : "Yes";
                    rezultatiview.Add(jb);

                    IzboriVM.RezultatiView jfk = new IzboriVM.RezultatiView();
                    jfk.IzbornaJedinica = item.IzbornaJedinica;
                    jfk.Kandidat = "John F. Kennedy";
                    jfk.BrojGlasova = item.JFK != null ? item.JFK.ToString() : "N/A";
                    jfk.Procenat = item.JFK != null ? ((Convert.ToDecimal(item.JFK) / ukupniGlasovi) * 100).ToString("0.00") + " %" : "N/A";
                    jfk.Error = item.JFK != null ? "" : "Yes";
                    rezultatiview.Add(jfk);

                    IzboriVM.RezultatiView jr = new IzboriVM.RezultatiView();
                    jr.IzbornaJedinica = item.IzbornaJedinica;
                    jr.Kandidat = "JR";
                    jr.BrojGlasova = item.JR != null ? item.JR.ToString()  : "N/A";
                    jr.Procenat = item.JR != null ? ((Convert.ToDecimal(item.JR) / ukupniGlasovi) * 100).ToString("0.00") + " %" : "N/A";
                    jr.Error = item.JR != null ? "" : "Yes";
                    rezultatiview.Add(jr);


                }
                IzboriVM model = new IzboriVM();
                model.IzborneJedinice = new List<string> { "New York", "Washington", "Texas" };
                model.rezultati = rezultatiview;
                return View(model);
            }
            return View();
        }
    }
}
