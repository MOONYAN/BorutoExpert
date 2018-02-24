using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class SwapQuerier : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.1;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 1000;

        [ExternVariable]
        public double TakeProfit = 1500;

        [ExternVariable]
        public int MagicNumber = 1;

        string _symbol;

        int _period;

        public override int init()
        {
            _symbol = Symbol();

            _period = Period();
            Console.WriteLine("OnInit");
            Console.WriteLine("Symbol:{0}  Period:{1}", _symbol, _period);
            Alert(_symbol + "on init");
            return base.init();
        }

        public override int start()
        {
            Console.WriteLine("----------------------------------------------");
            if (IsExistBuyPosition())
            {
                if (IsMatchColseBuyCondiction())
                {
                    //Console.WriteLine("CloseBuyPosition");
                    CloseBuyPosition();
                }
            }

            if (!IsExistBuyPosition())
            {
                if (IsMatchOpenBuyCondiction())
                {
                    //Console.WriteLine("OpenBuyPosition");
                    OpenBuyPosition();
                }
            }

            if (IsExistSellPosition())
            {
                if (IsMatchColseSellCondiction())
                {
                    //Console.WriteLine("CloseSellPosition");
                    CloseSellPosition();
                }
            }
            if (!IsExistSellPosition())
            {
                if (IsMatchOpenSellCondiction())
                {
                    //Console.WriteLine("OpenSellPosition");
                    OpenSellPosition();
                }
            }

            return base.start();
        }

        private bool IsExistBuyPosition()
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!base.OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) break;
                if (base.OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == OP_BUY)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsExistSellPosition()
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) break;
                if (OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == OP_SELL)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMatchOpenBuyCondiction()
        {
            return true;
        }

        private bool IsMatchOpenSellCondiction()
        {
            return true;
        }

        private bool IsMatchColseBuyCondiction()
        {
            return true;
        }

        private bool IsMatchColseSellCondiction()
        {
            return true;
        }

        private void OpenBuyPosition()
        {
            OrderSend(Symbol(), OP_BUY, Lots, Ask, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Blue);
        }

        private void OpenSellPosition()
        {
            OrderSend(Symbol(), OP_SELL, Lots, Bid, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Red);
        }

        private void CloseBuyPosition()
        {
            Console.WriteLine("Open Buy TimeOfDate:" + OrderOpenTime().DayOfWeek);
            Console.WriteLine("Close Buy TimeOfDate:" + Time[0].DayOfWeek);
            Console.WriteLine(OrderSwap());
            Console.WriteLine("                                                   ");
            OrderClose(OrderTicket(), OrderLots(), Bid, Slippage, Color.White);
        }

        private void CloseSellPosition()
        {
            Console.WriteLine("Open Sell TimeOfDate:" + OrderOpenTime().DayOfWeek);
            Console.WriteLine("Close Sell TimeOfDate:" + Time[0].DayOfWeek);
            Console.WriteLine(OrderSwap());
            Console.WriteLine("                                                   ");
            OrderClose(OrderTicket(), OrderLots(), Ask, Slippage, Color.White);
        }

        public override int deinit()
        {
            Console.WriteLine("OnDeinit");
            return base.deinit();
        }
    }
}
