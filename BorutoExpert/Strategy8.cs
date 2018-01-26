using NQuotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace BorutoExpert
{
    class Strategy8 : NQuotes.MqlApi
    {
        [ExternVariable]
        public double Lots = 0.01;

        [ExternVariable]
        public int Slippage = 30;

        [ExternVariable]
        public bool IsMatchBuyPrecondiction = false;

        [ExternVariable]
        public bool IsMatchSellPrecondiction = false;

        private int MagicNumber = 1;

        double MFIAgile => iMFI(_symbol, _period, 9, 1);

        double MFIAgileFormer => iMFI(_symbol, _period, 9, 2);

        double MFISmoose => iMFI(_symbol, _period, 14, 1);

        double MFISmooseFormer => iMFI(_symbol, _period, 14, 2);

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
            ProcessPrecondiction();
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

        private void ProcessPrecondiction()
        {
            if (!IsMatchBuyPrecondiction)
            {
                bool matchBuy = MFIAgile > 75 && MFISmoose > 75;
                IsMatchBuyPrecondiction = matchBuy;
                IsMatchSellPrecondiction = !matchBuy;
            }
            if (!IsMatchSellPrecondiction)
            {
                bool matchSell = MFIAgile < 25 && MFISmoose < 25;
                IsMatchBuyPrecondiction = !matchSell;
                IsMatchSellPrecondiction = matchSell;
            }
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
            if (IsMatchBuyPrecondiction)
            {
                bool isAgileInRange = 45 <= MFIAgile && MFIAgile <= 55;
                bool isSmooseInRange = 45 <= MFISmoose && MFISmoose <= 55;
                bool isMatchGold = MFIAgileFormer < MFISmooseFormer && MFIAgile > MFISmoose;
                return isAgileInRange && isSmooseInRange && isMatchGold;
            }
            return false;
        }

        private bool IsMatchOpenSellCondiction()
        {
            if (IsMatchSellPrecondiction)
            {
                bool isAgileInRange = 45 <= MFIAgile && MFIAgile <= 55;
                bool isSmooseInRange = 45 <= MFISmoose && MFISmoose <= 55;
                bool isMatchDead = MFIAgileFormer > MFISmooseFormer && MFIAgile < MFISmoose;
                return isAgileInRange && isSmooseInRange && isMatchDead;
            }
            return false;
        }

        private bool IsMatchColseBuyCondiction()
        {
            return MFIAgile > 75 && MFISmoose > 75;
        }

        private bool IsMatchColseSellCondiction()
        {
            return MFIAgile < 25 && MFISmoose < 25;
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