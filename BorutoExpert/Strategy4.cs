using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class Strategy4 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.1;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 0.300;

        [ExternVariable]
        public double TakeProfit = 1.000;

        private int MagicNumber = 1;

        double UpperBollinger => iBands(Symbol(), PERIOD_H4, 20, 2, 0, PRICE_CLOSE, MODE_UPPER, 1);

        double LowerBollinger => iBands(Symbol(), PERIOD_H4, 20, 2, 0, PRICE_CLOSE, MODE_LOWER, 1);

        double MA15 => iMA(Symbol(), PERIOD_H4, 15, 0, MODE_SMA, PRICE_CLOSE, 1);

        double MA20 => iMA(Symbol(), PERIOD_H4, 20, 0, MODE_SMA, PRICE_CLOSE, 1);

        double MA20B => iMA(Symbol(), PERIOD_H4, 20, 0, MODE_SMA, PRICE_CLOSE, 2);

        double MA75 => iMA(Symbol(), PERIOD_H4, 75, 0, MODE_SMA, PRICE_CLOSE, 1);

        double MA75B => iMA(Symbol(), PERIOD_H4, 75, 0, MODE_SMA, PRICE_CLOSE, 2);

        public override int start()
        {
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
            //return MA12 > MA60 && MA12 > MA120 && MA60 < MA120;
            return false;
        }

        private bool IsMatchOpenSellCondiction()
        {
            //double band1 = iBands(Symbol(), PERIOD_H4, 20, 2, 0, PRICE_CLOSE, MODE_LOWER, 1);
            //double band2 = iBands(Symbol(), PERIOD_H4, 20, 2, 0, PRICE_CLOSE, MODE_LOWER, 2);
            //return Close[1] <= band1 && Close[2] <= band2;
            return MA20 < MA75 && MA20B > MA75B;
        }

        private bool IsMatchColseBuyCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (Bid - OrderOpenPrice()));
            //Console.WriteLine("--------------StopLoss:" + (Bid - OrderOpenPrice()));
            //if (Bid - OrderOpenPrice() >= TakeProfit) return true;
            //if (Bid - OrderOpenPrice() <= -StopLoss) return true;
            //return MA12 < MA60;
            return true;
        }

        private bool IsMatchColseSellCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (OrderOpenPrice() - Ask));
            //Console.WriteLine("--------------StopLoss:" + (OrderOpenPrice() - Ask));

            if (OrderOpenPrice() - Ask >= TakeProfit)
            {
                Console.WriteLine("----OrderOpen:{0}  Ask:{1}  Profit:{2}", OrderOpenPrice(), Ask, (OrderOpenPrice() - Ask)*1000);
                return true;
            }
            /*if (OrderOpenPrice() - Ask <= -StopLoss)
            {
                Console.WriteLine("**OrderOpen:{0}  Ask:{1}  Loss:{2}", OrderOpenPrice(), Ask, (OrderOpenPrice() - Ask)*1000);
                return true;
            }*/
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
