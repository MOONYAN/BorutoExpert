using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class Strategy3 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.1;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 0.005;

        [ExternVariable]
        public double TakeProfit = 0;

        private int MagicNumber = 1;

        int Exponent => (int)MarketInfo(Symbol(), MODE_DIGITS);

        double UpperBollinger => iBands(Symbol(), PERIOD_M30, 12, 2, 0, PRICE_CLOSE, MODE_UPPER, 1);

        double LowerBollinger => iBands(Symbol(), PERIOD_M30, 12, 2, 0, PRICE_CLOSE, MODE_LOWER, 1);

        double SAR => iSAR(Symbol(), PERIOD_M30, 0.02, 0.2, 1);

        double MA12 => iMA(Symbol(), PERIOD_M30, 12, 0, MODE_SMA, PRICE_CLOSE, 1);

        double MA60 => iMA(Symbol(), PERIOD_M30, 60, 0, MODE_SMA, PRICE_CLOSE, 1);

        double MA120 => iMA(Symbol(), PERIOD_M30, 120, 0, MODE_SMA, PRICE_CLOSE, 1);

        void PrintHour() => Console.WriteLine("H:" + Time[0].Hour);
        void PrintMinute() => Console.WriteLine("M:" + Time[0].Minute);

        public override int start()
        {
            //Console.WriteLine(Time[0].Hour);
            //Console.WriteLine("High[0]:" + High[0]);
            //Console.WriteLine("Open[0]" + Open[0]);
            //Console.WriteLine("Colse[0]" + Close[0]);
            //Console.WriteLine("Low[0]:" + Low[0]);

            if (IsExistBuyPosition() && IsMatchColseBuyCondiction())
            {
                //Console.WriteLine("CloseBuyPosition");
                CloseBuyPosition();
            }

            if (!IsExistBuyPosition() && IsMatchOpenBuyCondiction())
            {
                //Console.WriteLine("OpenBuyPosition");
                OpenBuyPosition();
            }

            if (IsExistSellPosition() && IsMatchColseSellCondiction())
            {
                //Console.WriteLine("CloseSellPosition");
                CloseSellPosition();
            }

            if (!IsExistSellPosition() && IsMatchOpenSellCondiction())
            {
                //Console.WriteLine("OpenSellPosition");
                OpenSellPosition();
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
            return MA12 > MA60 && MA12 > MA120 && MA60 < MA120;
        }

        private bool IsMatchOpenSellCondiction()
        {
            return MA12 < MA60 && MA12 < MA120 && MA60 > MA120;
        }

        private bool IsMatchColseBuyCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (Bid - OrderOpenPrice()));
            //Console.WriteLine("--------------StopLoss:" + (Bid - OrderOpenPrice()));
            //if (Bid - OrderOpenPrice() >= TakeProfit) return true;
            if (Bid - OrderOpenPrice() <= -StopLoss) return true;
            return MA12 < MA60;
        }

        private bool IsMatchColseSellCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (OrderOpenPrice() - Ask));
            //Console.WriteLine("--------------StopLoss:" + (OrderOpenPrice() - Ask));
            //if (OrderOpenPrice() - Ask >= TakeProfit) return true;
            if (OrderOpenPrice() - Ask <= -StopLoss) return true;
            return MA12 > MA60;
        }

        private void OpenBuyPosition()
        {
            if (Time[0].Hour <7) return;
            OrderSend(Symbol(), OP_BUY, Lots, Ask, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Blue);
        }

        private void OpenSellPosition()
        {
            if (Time[0].Hour <7) return;
            OrderSend(Symbol(), OP_SELL, Lots, Bid, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Red);
        }

        private void CloseBuyPosition()
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) break;
                if (OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == OP_BUY)
                {
                    OrderClose(OrderTicket(), OrderLots(), Bid, Slippage, Color.White);
                }
            }
        }

        private void CloseSellPosition()
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) break;
                if (OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == OP_SELL)
                {
                    OrderClose(OrderTicket(), OrderLots(), Ask, Slippage, Color.White);
                }
            }
        }
    }
}
