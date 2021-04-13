using System;
using HLab.Erp.Data;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Data
{
    public static class DbDeleteExtention
    {
        public static bool DeleteModel<T>(this IViewModel<T> viewModel, IDialogService navigationService, IDataService dbService, string deleteMessage=null, string caption=null)
        where T : class, IEntity
        {
            var e = viewModel.Model;
            try
            {
                    using (var transaction = dbService.GetTransaction())
                    {
                        dbService.Delete(e);
                        //db.SaveChanges();
                        if (string.IsNullOrEmpty(deleteMessage) ||
                            navigationService.ShowMessageYesNo(caption, deleteMessage, "Question"))

                            transaction.Done();
                        else
                        {
                            return false;
                        }
                    }
                


                //dbService.Execute(db =>
                //{
                //    db.Remove(e);
                //    db.SaveChanges();
                //});
                viewModel.Model = null;
            }
            catch(Exception)
            {
                navigationService.ShowMessageOk("Suppression Impossible", caption ?? "", "Error");
                return false;
            }
            return true;
            //e.Delete();
        }

    }
}
