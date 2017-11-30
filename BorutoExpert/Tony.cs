using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class Tony : NQuotes.MqlApi
    {
        int MagicNumber = 1;
        int ticket;

        //==========================參數介面=================================
        [ExternVariable]
        public double Lots = 0.1;
        [ExternVariable]
        public double  StopProfit=100;
        [ExternVariable]
        public double Target = 300;

        [ExternVariable]
        public int MA1p = 1;
        [ExternVariable]
        public int MA20p = 20;
        [ExternVariable]
        public int TimeFrame = MqlApi.PERIOD_H4;

        //==========================宣告區===================================
        double ma1 => iMA(Symbol(), TimeFrame, MA1p, 0, MqlApi.MODE_SMA, MqlApi.PRICE_CLOSE, 0);
        double ma20 => iMA(Symbol(), TimeFrame, MA20p, 0, MqlApi.MODE_SMA, MqlApi.PRICE_CLOSE, 0);

        //==========================主程式===================================
        public override int start()
        {            
            CloseSellTicket();
            CloseBuyTicket();
            bsituat();
            ssituat();
            SellTicket();
            BuyTicket();

            return (0);
        }

        //==========================進單模組=================================== 

        //--------------------------BUY單函式----------------------------------   
        int bsituat()
        {
            int state = 0;
            int bstate = 0;
            if (OrdersTotal() > 0)
            {
                for (int i = OrdersTotal(); i >= 0; i--)
                {
                    if (OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
                    {
                        if (OrderSymbol() == Symbol() && OrderType() == OP_BUY && OrderComment() == "My order") { state = 1; }

                    }

                }
            }
            bstate = state;
            return bstate;
        }

        //--------------------------BUY單函式---------------------------------- 
        double bprice()
        {
            double price = 0;
            if (OrdersTotal() > 0)
            {
                for (int i = OrdersTotal(); i >= 0; i--)
                {
                    if (OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
                    {
                        if (OrderSymbol() == Symbol() && OrderType() == OP_BUY && OrderComment() == "My order") { price = OrderOpenPrice(); }
                    }

                }
            }
            return price;
        }
        //--------------------------SELL單函式----------------------------------  
        int ssituat()
        {
            int state = 0;
            int sstate = 0;
            if (OrdersTotal() > 0)
            {
                for (int i = OrdersTotal(); i >= 0; i--)
                {
                    if (OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
                    {
                        if (OrderSymbol() == Symbol() && OrderType() == OP_SELL && OrderComment() == "My order") { state = 1; }

                    }

                }
            }
            sstate = state;
            return sstate;
        }

        //--------------------------SELL單函式---------------------------------- 
        double sprice()
        {
            double price = 0;
            if (OrdersTotal() > 0)
            {
                for (int i = OrdersTotal(); i >= 0; i--)
                {
                    if (OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
                    {
                        if (OrderSymbol() == Symbol() && OrderType() == OP_SELL && OrderComment() == "My order") { price = OrderOpenPrice(); }
                    }

                }
            }
            return price;
        }



        //========================結單模組===============================
        //--------------------------目標價位---------------------------------- 
        double spricetarget()
        {
            double pricetarget = 0;
            if (sprice() != 0) { pricetarget = sprice() - Target; }
            return pricetarget;
        }

        double bpricetarget()
        {
            double pricetarget = 0;
            if (bprice() != 0) { pricetarget = bprice() + Target; }
            return pricetarget;
        }

        //--------------------------BUY單平倉----------------------------------    
        void CloseBuyTicket()
        {
            if (OrdersTotal() > 0)
            {
                if (Bid > bpricetarget() && bprice() != 0)
                {
                    for (int o = OrdersTotal(); o >= 0; o--)
                    {
                        if (OrderSelect(o, SELECT_BY_POS, MODE_TRADES))
                        {
                            if (OrderSymbol() == Symbol() && OrderType() == OP_BUY && OrderComment() == "My order") { bool closed = OrderClose(OrderTicket(), OrderLots(), Bid, 300, Color.White); }

                        }
                    }
                }
            }
        }

        //--------------------------SELL單平倉---------------------------------- 
        void CloseSellTicket()
        {
            if (OrdersTotal() > 0)
            {
                if (Ask < spricetarget() && sprice() != 0)
                {
                    for (int i = OrdersTotal(); i >= 0; i--)
                    {
                        if (OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
                        {
                            if (OrderSymbol() == Symbol() && OrderType() == OP_SELL && OrderComment() == "My order") { bool closed = OrderClose(OrderTicket(), OrderLots(), Ask, 300, Color.White); }

                        }
                    }
                }
            }

        }
        //+------------------------------------------------------------------+
        void BuyTicket()
        {
            if (bsituat() == 0)
            {
                if (ma1 > ma20) { ticket = OrderSend(Symbol(), OP_BUY, Lots, Ask, 300, 0, 0, "My order", MagicNumber, DateTime.MinValue, Color.Blue); }
            }
        }
        void SellTicket()
        {
            if (ssituat() == 0)
            {
                if (ma1 < ma20) { ticket = OrderSend(Symbol(), OP_SELL, Lots, Bid, 300, 0, 0, "My order", MagicNumber, DateTime.MinValue, Color.Red); }
            }
        }
    }
}
