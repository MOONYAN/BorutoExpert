using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    public class Strategy10 : MqlApi
    {
        [ExternVariable]
        public int MagicNumber = 1;

        [ExternVariable]
        public double Lots = 0.01;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 500;

        [ExternVariable]
        public double TakeProfit = 350;

        [ExternVariable]
        public int FatPeriod = 13;

        [ExternVariable]
        public double FatDeviation = 3.4;

        [ExternVariable]
        public bool IsTouchLowerBound = false;

        [ExternVariable]
        public bool IsTouchUpperBound = false;

        double LastSAR => iSAR(_symbol, _period, 0.01, 0.1, 1);

        double CurrentSAR => iSAR(_symbol, _period, 0.01, 0.1, 0);

        double LastClose => iClose(_symbol, _period, 1);

        double CurrentClose => iClose(_symbol, _period, 0);

        //double BollingUpper => iBands(_symbol, PERIOD_M30, FatPeriod, FatDeviation, 0, PRICE_CLOSE, MODE_UPPER, 0);

        //double BollingLower => iBands(_symbol, PERIOD_M30, FatPeriod, FatDeviation, 0, PRICE_CLOSE, MODE_LOWER, 0);

        string _symbol;

        int _period;

        public override int init()
        {
            _symbol = Symbol();
            _period = PERIOD_M30;
            Console.WriteLine("Symbol:{0}  Period:{1}", _symbol, _period);
            return base.init();
        }

        public override int start()
        {
            ProcessBuyPending();
            ProcessSellPending();
            ProcessPosition();
            return base.start();
        }

        private void ProcessPosition()
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!base.OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) continue;
                if (base.OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == OP_BUY)
                {
                    double price = OrderOpenPrice();
                    switch (OrderType())
                    {
                        case OP_BUY:
                            OrderModify(OrderTicket(), price, price - StopLoss * Point, price + TakeProfit * Point, DateTime.MinValue);
                            break;
                        case OP_SELL:
                            OrderModify(OrderTicket(), price, price + StopLoss * Point, price - TakeProfit * Point, DateTime.MinValue);
                            break;
                    }
                }
            }
        }

        private void ProcessBuyPending()
        {
            int ticket = OrderSend(_symbol, OP_BUYSTOP, Lots, CurrentSAR, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Blue);
            if (ticket != 0)
            {
                FilterPending(ticket, OP_BUYSTOP);
            }
            else
            {
                Console.WriteLine(ErrorDescription(GetLastError()));
            }
        }

        private void ProcessSellPending()
        {
            int ticket = OrderSend(_symbol, OP_SELLSTOP, Lots, CurrentSAR, Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Red);
            if (ticket != 0)
            {
                FilterPending(ticket, OP_SELLSTOP);
            }
            else
            {
                Console.WriteLine(ErrorDescription(GetLastError()));
            }
        }

        private void FilterPending(int ticket, int type)
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!base.OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) break;
                if (base.OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == type && OrderTicket() != ticket)
                {
                    OrderDelete(OrderTicket(),Color.Black);
                }
            }
        }

        public override int deinit()
        {
            return base.deinit();
        }
    }
}