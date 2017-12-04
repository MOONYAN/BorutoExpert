using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NQuotes;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class BGI1 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.01;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 30.00;

        [ExternVariable]
        public double TakeProfit = 50.00;

        private int MagicNumber = 1;

        int Exponent => (int)MarketInfo(Symbol(), MODE_DIGITS);

        double UpperBollinger => iBands(Symbol(), PERIOD_D1, 20, 2, 0, PRICE_CLOSE, MODE_UPPER, 1);

        double LowerBollinger => iBands(Symbol(), PERIOD_D1, 20, 2, 0, PRICE_CLOSE, MODE_LOWER, 1);

        //double SAR => iSAR(Symbol(), PERIOD_D1, 0.02, 0.2, 1);

        double MA59 => iMA(Symbol(), PERIOD_D1, 59, 0, MODE_SMA, PRICE_CLOSE, 1);

        public override int start()
        {
            //Console.WriteLine("start");
            //Console.WriteLine("High[0]:" + High[0]);
            //Console.WriteLine("Open[0]" + Open[0]);
            //Console.WriteLine("Colse[0]" + Close[0]);
            //Console.WriteLine("Low[0]:" + Low[0]);

            if (IsExistBuyPosition() && IsMatchColseBuyCondiction())
            {
                Console.WriteLine("CloseBuyPosition");
                CloseBuyPosition();
            }

            if (!IsExistBuyPosition() && IsMatchOpenBuyCondiction())
            {
                Console.WriteLine("OpenBuyPosition");
                OpenBuyPosition();
            }

            if (IsExistSellPosition() && IsMatchColseSellCondiction())
            {
                Console.WriteLine("CloseSellPosition");
                CloseSellPosition();
            }

            if (!IsExistSellPosition() && IsMatchOpenSellCondiction())
            {
                Console.WriteLine("OpenSellPosition");
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
            return Close[1] >= UpperBollinger && Low[1] < MA59 && Close[1] > MA59;
        }

        private bool IsMatchOpenSellCondiction()
        {
            return Close[1] <= LowerBollinger && High[1] > MA59 && Close[1] < MA59;
        }

        private bool IsMatchColseBuyCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (Bid - OrderOpenPrice()));
            //Console.WriteLine("--------------StopLoss:" + (Bid - OrderOpenPrice()));
            if (Bid - OrderOpenPrice() >= TakeProfit) return true;
            if (Bid - OrderOpenPrice() <= -StopLoss) return true;
            return false;
        }

        private bool IsMatchColseSellCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (OrderOpenPrice() - Ask));
            //Console.WriteLine("--------------StopLoss:" + (OrderOpenPrice() - Ask));
            if (OrderOpenPrice() - Ask >= TakeProfit) return true;
            if (OrderOpenPrice() - Ask <= -StopLoss) return true;
            return false;
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
