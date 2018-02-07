using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    class Strategy9 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.1;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public double StopLoss = 1000;

        [ExternVariable]
        public double TakeProfit = 1500;

        private int MagicNumber = 1;

        double SARLong => iSAR(_symbol, _period, 0.002, 0.2, 0);

        double SARLongFormer => iSAR(_symbol, _period, 0.002, 0.2, 1);

        double SARMid => iSAR(_symbol, _period, 0.005, 0.2, 0);

        double SARMidFormer => iSAR(_symbol, _period, 0.005, 0.2, 1);

        double SARShort => iSAR(_symbol, _period, 0.008, 0.2, 0);

        double SARShortFormer => iSAR(_symbol, _period, 0.008, 0.2, 1);

        double MALong => iMA(_symbol, _period, 568, 0, MODE_EMA, PRICE_CLOSE, 0);

        double MALongFormer => iMA(_symbol, _period, 568, 0, MODE_EMA, PRICE_CLOSE, 1);

        double MAMid => iMA(_symbol, _period, 65, 0, MODE_EMA, PRICE_CLOSE, 0);

        double MAMidFormer => iMA(_symbol, _period, 65, 0, MODE_EMA, PRICE_CLOSE, 1);

        double MAShort => iMA(_symbol, _period, 30, 0, MODE_EMA, PRICE_CLOSE, 0);

        double MAShortFormer => iMA(_symbol, _period, 30, 0, MODE_EMA, PRICE_CLOSE, 1);

        double BollingerUpper => iBands(_symbol, _period, 22, 2, 0, PRICE_CLOSE, MODE_UPPER, 1);

        double BollingerLower => iBands(_symbol, _period, 22, 2, 0, PRICE_CLOSE, MODE_LOWER, 1);

        string _symbol;

        int _period;

        double _factor;

        public override int init()
        {
            _symbol = Symbol();
            _factor = Math.Pow(10, -Digits);
            _period = Period();
            Console.WriteLine("OnInit");
            Console.WriteLine("Symbol:{0}  Factor:{1}  Period:{2}", _symbol, _factor, _period);
            ReportPosition();
            return base.init();
        }

        private void ReportPosition()
        {
            for (int i = 0, total = OrdersTotal(); i < total; i++)
            {
                if (!base.OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) break;
                Console.WriteLine("Magic:{0}  Symbol:{1}  Type:{2} Profit:{3} Ticket:{4}", OrderMagicNumber(), OrderSymbol(), OrderType(), OrderProfit(), OrderTicket());
            }
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
            bool isLastSarUp = SARLongFormer >= Open[1] && SARMidFormer >= Open[1] && SARShort >= Open[1];
            bool isCurrentSarDown = SARLong <= Open[0] && SARMid <= Open[0] && SARShort <= Open[0];
            bool isMatchMA1 = MAShort > MAMid && MAShortFormer <= MAMidFormer;
            bool isMatchMA2 = MAMid > MALong && MAMidFormer <= MALongFormer;
            return isLastSarUp && isCurrentSarDown && isMatchMA1 || isCurrentSarDown && isMatchMA2;
        }

        private bool IsMatchOpenSellCondiction()
        {
            bool isLastSarDown = SARLongFormer <= Open[1] && SARMidFormer <= Open[1] && SARShort <= Open[1];
            bool isCurrentSarDown = SARLong >= Open[0] && SARMid >= Open[0] && SARShort >= Open[0];
            bool isMatchMA1 = MAShort < MAMid && MAShortFormer >= MAMidFormer;
            bool isMatchMA2 = MAMid < MALong && MAMidFormer >= MALongFormer;
            return isLastSarDown && isCurrentSarDown && isMatchMA1 || isCurrentSarDown && isMatchMA2;
        }

        private bool IsMatchColseBuyCondiction()
        {
            bool isMatchSar = SARMid >= Open[0];
            bool isMatchMA = High[0] >= MALong && Low[0] <= MALong;
            return isMatchSar || isMatchMA;
        }

        private bool IsMatchColseSellCondiction()
        {
            bool isMatchSar = SARMid <= Open[0];
            bool isMatchMA = High[0] >= MALong && Low[0] <= MALong;
            return isMatchSar || isMatchMA;
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
            Console.WriteLine("OnDeinit");
            return base.deinit();
        }
    }
}
