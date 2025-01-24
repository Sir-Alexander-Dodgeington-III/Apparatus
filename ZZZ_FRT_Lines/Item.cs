namespace ZZZ_FRT_Lines
{
    public class lineItems
    {
        public string id { get; set; }
        public string dateTime { get; set; }
        public string document { get; set; }
        public string branch { get; set; }
        public string acct { get; set; }
        public string qty { get; set; }
        public string unit { get; set; }
        public string trCost { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ dateTime }\t{ document }\t{ branch }\t{ acct }\t{ qty }\t{ unit }\t{ trCost }";
            }
        }

    }
}

namespace BR60generated
{
    public class addItems
    {
        public string add { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ add }";
            }
        }

    }
}


namespace PartPerDay_Form
{
    public class ProdlineItems
    {
        public string Prefix { get; set; }
        public string Branch { get; set; }
        public string PG { get; set; }
        public string Part { get; set; }
        public string Qty { get; set; }
        public string datetime { get; set; }
        public string RC { get; set; }



        public string ProdInfo
        {
            get
            {
                return $"{ Prefix }\t{ Branch }\t{ PG }\t{ Part }\t{ Qty }\t{ datetime }\t{ RC }";
            }
        }

    }
}



namespace Fuse5Export
{
    public class Fuse5lineItems
    {
        public string PG { get; set; }
        public string Customer { get; set; }


        public string Fuse5Info
        {
            get
            {
                return $"{ PG }\t{ Customer }";
            }
        }

    }
}


namespace WixForm
{
    public class WixlineItems
    {
        public string Branch { get; set; }
        public string Customer { get; set; }
        public string Name { get; set; }
        public string PartNumber { get; set; }
        public string Desc { get; set; }
        public string Document { get; set; }
        public string Writer { get; set; }
        public string Date { get; set; }
        public string Qty { get; set; }
        public string Sales { get; set; }
        public string UnitPrice { get; set; }
        public string P8 { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }


        public string WixInfo
        {
            get
            {
                return $"{ Branch }\t{ Customer }\t{ Name }\t{ PartNumber }\t{ Desc }\t{ Document }\t{ Writer }\t{ Date }\t{ Qty }\t{ Sales }\t{ UnitPrice }\t{ P8 }";
            }
        }

    }
}


namespace OnHandOnPO_Form
{
    public class OnHandOnPO_Items
    {
        public string Part { get; set; }
        public int Free { get; set; }
        public int QtyOnPO { get; set; }
        public int Qt_Qty { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int Total { get; set; }
        public string CIND { get; set; }
        public string Locn { get; set; }
        public string WMSSort { get; set; }
        public string PickQty { get; set; }

        public string ProdInfo
        {
            get
            {
                return $"{ Part }\t{ Free }\t{ Qt_Qty }\t{ QtyOnPO }\t{PickQty}\t{ Total }\t{ Min }\t{ Max }\t{ CIND }\t{ Locn }\t{WMSSort}";
            }
        }

    }
}


namespace PaintVsSandpaper_Form
{
    public class lineItems
    {
        public string Branch { get; set; }
        public string Acct { get; set; }
        public string Name { get; set; }
        public string Rep { get; set; }
        public string RepName { get; set; }
        public string Email { get; set; }
        public string PaintSales { get; set; }
        public string SPSales { get; set; }
        public string PercSales { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ Branch }\t{ Acct }\t{ Name }\t{ Rep }\t{ RepName }\t{ Email }\t{ PaintSales }\t{ SPSales }\t{ PercSales }";
            }
        }

    }
}

namespace Login_Form
{
    public class addItems
    {
        public string add { get; set; }
        public string add1 { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ add }";
            }
        }

    }
}


namespace wmsSort
{
    public class Items
    {
        public string PG { get; set; }

        public string FullInfo
        {
            get
            {
                return $"{ PG }";
            }
        }

    }
}

namespace AutologuePricing
{
    public class ALItems
    {
        public string PG { get; set; }


        public string FULLInfo
        {
            get
            {
                return $"{ PG }";
            }
        }
    }
}
