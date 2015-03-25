using System;
using System.Collections.Generic;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Views;
using Hoe.Basic.AppLogic;
using Hoe.Basic.View;
using Hoe.Basic.Model;
using Hoe.Basic.AppLogic.Event;
using System.Windows.Forms;
using System.Linq;

namespace Hoe.Basic.Controller
{
    public class ProductController : ControllerBase
    {
        public override ITask Task
        {
            get { return base.Task; }
            set
            {
                base.Task = value;
            }
        }

        public override IView View
        {
            get {return base.View; }
            set
            {
                base.View = value;
            }
        }

        public void NewSemiProduct(SemiProduct semiproduct)
        {

            List<SemiProduct> specificSemiProduct = (from p in (Task as ProductTask).SalesTask.SemiProducts
                                             where p.Equals(semiproduct)
                                             select p).ToList();

            if (specificSemiProduct.Count > 0)
            {
                MessageBox.Show("已经存在这个日期的这批货物啦\n\r如果你想增加库存数量，则直接双击修改即可");
                return;
            }

            SaleTask task = (Task as ProductTask).SalesTask;
            task.SemiProducts.Add(semiproduct);
            task.TriggerRepoSemiProductsChanged(this, new SemiProductChangeEventArg(semiproduct, ModelChangeEventArg.INSERT));
        }
        
    }
}
