using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    class Strategy7 : NQuotes.MqlApi
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

        double SARLong => iSAR(_symbol, _period, 0.002, 0.2, 1);

        double SARMid => iSAR(_symbol, _period, 0.005, 0.2, 1);

        double SARShort => iSAR(_symbol, _period, 0.008, 0.2, 1);

        double MACDMediumMain => iMACD(_symbol, _period, 12, 26, 9, PRICE_MEDIAN, MODE_MAIN, 1);

        double MACDMediumSignal => iMACD(_symbol, _period, 12, 26, 9, PRICE_MEDIAN, MODE_SIGNAL, 1);

        double MACDCloseMain => iMACD(_symbol, _period, 12, 26, 9, PRICE_CLOSE, MODE_MAIN, 1);

        double MACDCloseSignal => iMACD(_symbol, _period, 12, 26, 9, PRICE_CLOSE, MODE_SIGNAL, 1);

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
                Console.WriteLine("Magic:{0}  Symbol:{1}  Type:{2}", OrderMagicNumber(), OrderSymbol(), OrderType());
            }
        }

        public override int start()
        {
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
            bool isMatchSar = SARLong <= SARMid && SARMid <= SARShort && SARShort <= Close[1];
            bool isMatchMACD = MACDCloseMain > MACDMediumMain;
            bool nonBetrayMACD = MACDCloseMain > MACDMediumSignal && MACDMediumMain > MACDMediumSignal;
            return isMatchSar && isMatchMACD && nonBetrayMACD;
        }

        private bool IsMatchOpenSellCondiction()
        {
            bool isMatchSar = SARLong >= SARMid && SARMid >= SARShort && SARShort >= Close[1];
            bool isMatchMACD = MACDCloseMain < MACDMediumMain;
            bool nonBetrayMACD = MACDCloseMain < MACDMediumSignal && MACDMediumMain < MACDMediumSignal;
            return isMatchSar && isMatchMACD && nonBetrayMACD;
        }

        private bool IsMatchColseBuyCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (Bid - OrderOpenPrice()));
            //Console.WriteLine("--------------StopLoss:" + (Bid - OrderOpenPrice()));
            //if (Bid - OrderOpenPrice() >= TakeProfit * _factor) return true;
            //if (Bid - OrderOpenPrice() <= -StopLoss * _factor) return true;
            bool isMatchMACD = MACDCloseMain > MACDMediumMain;
            return !isMatchMACD;
        }

        private bool IsMatchColseSellCondiction()
        {
            //Console.WriteLine("------------------------TakeProfit:" + (OrderOpenPrice() - Ask));
            //Console.WriteLine("--------------StopLoss:" + (OrderOpenPrice() - Ask));
            //if (OrderOpenPrice() - Ask >= TakeProfit * _factor) return true;
            //if (OrderOpenPrice() - Ask <= -StopLoss * _factor) return true;
            bool isMatchMACD = MACDCloseMain < MACDMediumMain;
            return !isMatchMACD;
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

        public override int deinit()
        {
            Console.WriteLine("OnDeinit");
            return base.deinit();
        }
    }
}
