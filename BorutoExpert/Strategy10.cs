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
        public double Step = 0.01;

        [ExternVariable]
        public double Maximum = 0.1;

        [ExternVariable]
        public int TimeFrame = PERIOD_M30;

        double LastSAR => iSAR(_symbol, _period, 0.01, 0.1, 1);

        double CurrentSAR => iSAR(_symbol, _period, 0.01, 0.1, 0);

        string _symbol;

        int _period;

        public override int init()
        {
            _symbol = Symbol();
            _period = TimeFrame;
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

            int ticket = OrderSend(_symbol, OP_BUYSTOP, Lots, NormalizeDouble(CurrentSAR, Digits), Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Blue);
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
            int ticket = OrderSend(_symbol, OP_SELLSTOP, Lots, NormalizeDouble(CurrentSAR, Digits), Slippage, 0, 0, "", MagicNumber, DateTime.MinValue, Color.Red);
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
                if (!base.OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) continue;
                if (base.OrderMagicNumber() == MagicNumber && OrderSymbol() == Symbol() && OrderType() == type && OrderTicket() != ticket)
                {
                    OrderDelete(OrderTicket(), Color.Black);
                }
            }
        }

        public override int deinit()
        {
            Console.WriteLine("Fucking deinit");
            return base.deinit();
        }
    }
}