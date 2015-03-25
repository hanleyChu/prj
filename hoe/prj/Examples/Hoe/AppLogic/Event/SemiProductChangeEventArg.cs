using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hoe.Basic.Model;

namespace Hoe.Basic.AppLogic.Event
{
    public class SemiProductChangeEventArg : ModelChangeEventArg
    {
        public SemiProductChangeEventArg(SemiProduct changedSemiProduct, int changingType)
        {
            this.ChangedSemiProduct = changedSemiProduct;
            this.ChangingType = changingType;
        }

        public SemiProduct ChangedSemiProduct{set;get;}
    }
}
