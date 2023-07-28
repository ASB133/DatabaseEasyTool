using Dapper;
using DatabaseEasyTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DatabaseEasyTool.Controllers
{

    class ColumnModel
    {
        public string CName { get; set; }
        public string CType { get; set; }

        public override string ToString()
        {
            return CName+"--"+CType;
        }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync()
        {
            using (var connection = new SqlConnection("Data Source=sqlazewddamcilsesql99001.database.windows.net;Initial Catalog=ILSE;User ID=ILSE;Password=Spinazie44;MultipleActiveResultSets=true;Connect Timeout=30"))
            {
                await connection.OpenAsync();
                var tablse = await connection.QueryAsync<string>("SELECT table_name FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' and TABLE_SCHEMA='dbo'");

                

                foreach (string table in tablse.Where(t => !t.Contains("A_")))
                {
                  

                    var getcolumn_que = "SELECT COLUMN_NAME as CName,CAST(DATA_TYPE AS varchar) as CType FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + table+"' ORDER BY ORDINAL_POSITION";
                    Debug.WriteLine(table + "-------------------------------------");
                    var columns=await connection.QueryAsync<ColumnModel>(getcolumn_que);
                    try
                    {
                        foreach (ColumnModel column in columns)
                        {
                            if (column.CType.Contains("nvarchar"))
                            {

                                try
                                {
                                    var havedataquery = $"select top(5) * from {table} where {column.CName}='2007377062'";
                                    Debug.WriteLine(havedataquery);
                                    var rowss = await connection.QueryAsync(havedataquery);
                                    if (rowss.Count() > 0)
                                    {
                                        Debug.WriteLine(JsonConvert.SerializeObject(rowss.FirstOrDefault()));
                                        break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.ToString());
                                }
                               
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                    
                }
                var sss = 10;
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}