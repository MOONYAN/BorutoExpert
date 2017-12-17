using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class Strategy5 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.1;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 1.000;

        [ExternVariable]
        public double TakeProfit = 0;

        private int MagicNumber = 1;

        double MA30 => iMA(Symbol(), PERIOD_H1, 30, 0, MODE_SMA, PRICE_CLOSE, 1);

        double SARIn => iSAR(Symbol(), PERIOD_H1, 0.008, 0.1, 1);

        double SAROut => iSAR(Symbol(), PERIOD_H1, 0.05, 0.2, 1);

        bool BetweenTimeHours => Time[0].Hour >= 8 && Time[0].Hour < 18;

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
            bool sarReverse = SARIn < Low[1] && SAROut < Ask;
            bool goldMA = Close[1] > MA30;

            return sarReverse && goldMA;
        }

        private bool IsMatchOpenSellCondiction()
        {
            bool sarReverse = SARIn > High[1] && SAROut > Bid;
            bool deadMA = Close[1] < MA30;

            return sarReverse && deadMA;
        }

        private bool IsMatchColseBuyCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (Bid - OrderOpenPrice()));
            //Console.WriteLine("--------------StopLoss:" + (Bid - OrderOpenPrice()));
            //if (Bid - OrderOpenPrice() >= TakeProfit) return true;
            //if (Bid - OrderOpenPrice() <= -StopLoss) return true;
            //return MA12 < MA60;
            return SAROut > Bid;
        }

        private bool IsMatchColseSellCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (OrderOpenPrice() - Ask));
            //Console.WriteLine("--------------StopLoss:" + (OrderOpenPrice() - Ask));

            /*if (OrderOpenPrice() - Ask >= TakeProfit)
            {
                Console.WriteLine("----OrderOpen:{0}  Ask:{1}  Profit:{2}", OrderOpenPrice(), Ask, (OrderOpenPrice() - Ask) * 1000);
                return true;
            }*/
            /*if (OrderOpenPrice() - Ask <= -StopLoss)
            {
                Console.WriteLine("**OrderOpen:{0}  Ask:{1}  Loss:{2}", OrderOpenPrice(), Ask, (OrderOpenPrice() - Ask)*1000);
                return true;
            }*/
            return SAROut < Ask;
        }

        private void OpenBuyPosition()
        {
            if (!BetweenTimeHours) return;
            int result = OrderSend(Symbol(), OP_BUY, Lots, Ask, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Blue);
            OrderSelect(result, SELECT_BY_TICKET);
            OrderModify(OrderTicket(), OrderOpenPrice(), OrderOpenPrice() - StopLoss, 0, DateTime.MinValue, Color.Green);
            SendNotification("OpenBuy:" + Ask);
            Alert("OpenBuy:" + Ask);
        }

        private void OpenSellPosition()
        {
            if (!BetweenTimeHours) return;
            int result = OrderSend(Symbol(), OP_SELL, Lots, Bid, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Red);
            OrderSelect(result, SELECT_BY_TICKET);
            OrderModify(OrderTicket(), OrderOpenPrice(), OrderOpenPrice() + StopLoss, 0, DateTime.MinValue, Color.Green);
            SendNotification("OpenSell:" + Bid);
            Alert("OpenSell:" + Bid);
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
