﻿using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class Strategy1 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.01;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 0;

        [ExternVariable]
        public double TakeProfit = 0;

        private int MagicNumber = 1;

        double HighIregression => NormalizeDouble(iCustom(Symbol(), PERIOD_H4, "i-Regr", 1, 0), Convert.ToInt32(MarketInfo(OrderSymbol(), MODE_DIGITS)));

        double LowIregression => NormalizeDouble(iCustom(Symbol(), PERIOD_H4, "i-Regr", 2, 0), Convert.ToInt32(MarketInfo(OrderSymbol(), MODE_DIGITS)));

        double MidIregression => NormalizeDouble(iCustom(Symbol(), PERIOD_H4, "i-Regr", 0, 0), Convert.ToInt32(MarketInfo(OrderSymbol(), MODE_DIGITS)));

        public override int start()
        {
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
            return Low[1] < LowIregression;
        }

        private bool IsMatchOpenSellCondiction()
        {
            return High[1] > HighIregression;
        }

        private bool IsMatchColseBuyCondiction()
        {
            return High[0] >= MidIregression;
        }

        private bool IsMatchColseSellCondiction()
        {
            return Low[0] <= MidIregression;
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
