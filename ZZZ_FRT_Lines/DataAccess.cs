using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;


namespace ZZZ_FRT_Lines
{
    public class DataAccess
    {
        public List<lineItems> GetItems(string PG, string StartDate, string EndDate)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<lineItems>($"select iheads.datetime, iheads.document, iheads.branch, iheads.acct, ilines.Qty, ilines.unit, ilines.trcost from ilines (nolock)inner join iheads (nolock) on ilines.prefix = iheads.prefix and ilines.document = iheads.document and CAST(iheads.datetime as date) BETWEEN '{ StartDate }' and '{ EndDate }' inner join product(nolock) on product.keycode = ilines.part and ILines.PG = '{ PG }' ORDER BY DateTime").ToList();
                return outPut;
            }

        }
    }

}


namespace Fuse5Export
{
    public class DataAccess
    {
        public List<Fuse5lineItems> GetItems(string Customer)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<Fuse5lineItems>($"SELECT Keycode FROM Customer WHERE KeyCode = '{ Customer }'").ToList();
                return outPut;
            }

        }
    }

    public class DataAccess1
    {
        public List<Fuse5lineItems> GetItems(string PG)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<Fuse5lineItems>($"SELECT DISTINCT(PG) FROM Product WHERE PG = '{ PG }'").ToList();
                return outPut;
            }

        }
    }
}



//namespace WIXreport
//{
//    public class WixDataAccess
//    {
//        public List<WixlineItems> GetItems(string PG, string StartDate, string EndDate)
//        {
//            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
//            {
//                var outPut = connection.Query<WixlineItems>($"SELECT * FROM [AUTOPART].[dbo].[VW_MERI_I2_DoNotAlter] WHERE PG = '{ PG }' AND CAST(DateTime as date) BETWEEN '{ StartDate }' AND '{ EndDate }'").ToList();
//                return outPut;
//            }
//        }
//    }
//}

namespace BR60generated
{
    public class DataAccess1
    {
        public List<addItems> GetItems(string add)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<addItems>($"select TOP(1) * FROM Product WHERE PG = '{add}'").ToList();
                return outPut;
            }

        }
    }

    public class DataAccess2
    {
        public List<addItems> GetItems(string add)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<addItems>($"select TOP(1) * FROM Product WHERE Range = '{add}'").ToList();
                return outPut;
            }

        }
    }
}

namespace PaintVsSandpaper_Form
{
    public class DataAccess
    {
        public List<lineItems> GetItems()
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("ArnoldGroup")))
            {
                var outPut = connection.Query<lineItems>($@"                                
                                WITH CTE1 (Branch, Acct, Name, Rep) as (
                                SELECT Loc, CustNo, Customer, Salesperson
                                FROM CustomerMaster NOLOCK
                                GROUP BY Loc, CustNo, Customer, Salesperson),

                                CTE2 (Rep, Name, Email) as (
                                SELECT KeyCode, Name, Email
                                FROM         SalesRepInfo
                                GROUP BY KeyCode, Name, Email),

                                CTE3(Acct, Sales) as (
                                SELECT
                                CustNo,
                                SUM(NetSales + CoreSales) AS PaintSales
                                FROM SalesMaster NOLOCK
                                WHERE PeriodYear >= YEAR(DATEADD(YY, -1, GETDATE())) AND PeriodMonth <= MONTH(GETDATE()) AND Line IN('PPG','AKZ','BSF')
                                GROUP BY CustNo),

                                CTE4(Acct, Sales) as (
                                SELECT
                                CustNo,
                                SUM(NetSales + CoreSales) AS SandPaperSales
                                FROM SalesMaster NOLOCK
                                WHERE PeriodYear >= YEAR(DATEADD(YY, -1, GETDATE())) AND PeriodMonth <= MONTH(GETDATE()) AND Line IN('MMM','NOR')
                                GROUP BY CustNo)

                                SELECT 'BR' + I.Branch as Branch, I.Acct, I.Name, II.Rep, II.Name, II.Email, III.Sales as PaintSales, IV.Sales as SandpaperSales, 
                                CASE
                                WHEN IV.Sales > 0 AND III.Sales< 0 THEN NULLIF(IV.Sales,0)/NULLIF(III.Sales* -1,0) * 100 
                                ELSE NULLIF(IV.Sales,0)/NULLIF(III.Sales,0) * 100 END
                                as PercSales
                                FROM CTE1 I INNER JOIN
                                CTE2 II ON I.Rep = II.Rep INNER JOIN
                                CTE3 III ON I.Acct = III.Acct INNER JOIN
                                CTE4 IV ON I.Acct = IV.Acct
                                WHERE CASE
                                WHEN IV.Sales > 0 AND III.Sales< 0 THEN NULLIF(IV.Sales,0)/NULLIF(III.Sales* -1,0) * 100 
                                ELSE NULLIF(IV.Sales,0)/NULLIF(III.Sales,0) * 100 END< 25
                                GROUP BY I.Branch, I.Acct, I.Name, II.Rep, II.Name, II.Email, III.Sales, IV.Sales
                                ORDER BY I.Branch, II.Rep"
                        ).ToList();
                return outPut;
            }

        }
    }
}

namespace Login_Form
{
    public class DataAccess
    {
    public List<addItems> GetItems(string add)
        
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<addItems>($"SELECT TOP(1) * FROM Codes WHERE Prefix = 'O' AND KeyCode = '{add}'").ToList();
                return outPut;
            }

        }
    }

    public class DataAccess1
    {
        public List<addItems> GetItems(string add, string add1)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<addItems>($"SELECT TOP(1) * FROM Codes WHERE Prefix = 'O' AND a3 = '{add}' AND KeyCode = '{add1}'").ToList();
                return outPut;
            }

        }
    }
}

namespace wmsSort
{
        public class DataAccess0
    {
        public List<Items> GetItems(string PG)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<Items>($"SELECT TOP(1) * FROM Product WHERE PG = '{ PG }'").ToList();
                return outPut;
            }

        }
    }
    //public class DataAccess
    //{
    //    public List<Items> GetItems()
    //    {
    //        using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
    //        {
    //            var outPut = connection.Query<Items>($"SELECT SubKey FROM MERI_WMSSort WHERE Prefix = 'L'").ToList();
    //            return outPut;
    //        }

    //    }
    //}
    //public class DataAccess1
    //{
    //    public List<Items> GetItems(string subkey)
    //    {
    //        using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
    //        {
    //            var outPut = connection.Query<Items>($"SELECT a1 FROM MERI_WMSSort WHERE Prefix = 'P' AND SubKey = '{ subkey }'").ToList();
    //            return outPut;
    //        }

    //    }
    //}
    //public class DataAccess2
    //{
    //    public List<Items> GetItems(string subkey)
    //    {
    //        using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
    //        {
    //            var outPut = connection.Query<Items>($"SELECT a1, Seq1, Just1, Seq2, Just2, Seq3, Just3, Seq4, Just4, Seq5, Just5, Seq6, Just6 FROM MERI_WMSSort WHERE Prefix = 'S' AND SubKey = '{ subkey }'").ToList();
    //            return outPut;
    //        }

    //    }
    //}
}

namespace AutologuePricing
{

    public class DataAccess
    {
        public List<ALItems> GetItems(string PG)
        {
            using (IDbConnection connection = new SqlConnection(Helper.ConnString("AUTOPART")))
            {
                var outPut = connection.Query<ALItems>($"SELECT TOP(1) * FROM Product WHERE PG = '{ PG }'").ToList();
                return outPut;
            }

        }
    }
}







