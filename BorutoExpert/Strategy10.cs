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
        public double Lots = 0.1;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 1000;

        [ExternVariable]
        public double TakeProfit = 1500;

        [ExternVariable]
        public int FatPeriod = 21;

        [ExternVariable]
        public double FatDeviation = 3.4;

        [ExternVariable]
        public int ThinPeriod = 21;

        [ExternVariable]
        public double ThinDerivation = 1.3;        

        double BollingUpperFat => iBands(_symbol, _period, FatPeriod, FatDeviation, 0, PRICE_CLOSE, MODE_UPPER, 0);

        double BollingLowerFat => iBands(_symbol, _period, FatPeriod, FatDeviation, 0, PRICE_CLOSE, MODE_LOWER, 0);

        double BollingUpperThin => iBands(_symbol, _period, ThinPeriod, ThinDerivation, 0, PRICE_CLOSE, MODE_UPPER, 0);

        double BollingLowerThin => iBands(_symbol, _period, ThinPeriod, ThinDerivation, 0, PRICE_CLOSE, MODE_LOWER, 0);

        string _symbol;

        int _period;

        public override int init()
        {
            _symbol = Symbol();
            _period = PERIOD_M15;
            Console.WriteLine("Symbol:{0}  Period:{1}", _symbol, _period);
            return base.init();
        }

        public override int start()
        {
            if (IsExistBuyPosition())
            {
                if (IsMatchColseBuyCondiction())
                {
                    //Console.WriteLine("CloseBuyPosition");
                    CloseBuyPosition();
                }
            }
            else
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
            else
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
            return Ask <= BollingLowerFat;
        }

        private bool IsMatchOpenSellCondiction()
        {
            return Bid >= BollingUpperFat;
        }

        private bool IsMatchColseBuyCondiction()
        {
            return Bid >= BollingLowerThin;
        }

        private bool IsMatchColseSellCondiction()
        {
            return Ask <= BollingUpperThin;
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
            OrderClose(OrderTicket(), OrderLots(), Bid, Slippage, Color.White);
        }

        private void CloseSellPosition()
        {
            OrderClose(OrderTicket(), OrderLots(), Ask, Slippage, Color.White);
        }

        public override int deinit()
        {
            return base.deinit();
        }
    }
}