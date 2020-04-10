using System;
using System.Data.Entity;


namespace DataAccessLayer
{
    public class BaseRepository<TDbContext> : IDisposable
          where TDbContext : DbContext
    {


        protected TDbContext _dbContext = null;

        public BaseRepository(TDbContext context)
        {
            _dbContext = context;

        }
        public int SaveChanges()
        {
            if (_dbContext != null)
            {
                int x = _dbContext.SaveChanges();
                if (x != 0)
                {
                    Dispose(true);// 释放所有资源，不知道可不可以，因为这个函数本身是该对象的一部分，如果在这一步释放了所有资源那后面的return语句释放还能执行呢？这里释放非托管资源比较保险一点，到时候测试一下。                  
                }
                return x;
            }
            return 0;
        }
        /* 仅保存功能
                public int SaveChanges()
                {
                    if (_dbContext != null)
                    {
                      
                        return  _dbContext.SaveChanges();
                    }
                    return 0;
                }
                */
        /*  //果然是.net4.0没有此部分功能需.net4.5才能使用如下函数
          public Task<int> SaveChangesAsync()
          {
              if (_dbContext != null)
              {
                  return _dbContext.SaveChangesAsync();
              }
              return Task<int>.FromResult(0);
          }
          */

        #region "Disposable编程模式1"

        //private bool disposed = false;
        //        protected virtual void Dispose(bool disposing)
        //        {
        //            if (!this.disposed)
        //            {
        //                if (disposing)
        //                {
        //                    _dbContext.Dispose();
        //                }
        //            }
        //            this.disposed = true;
        //        }
        //        public void Dispose()
        //        {
        //            Dispose(true);
        //            GC.SuppressFinalize(this);
        //        }

        #endregion
        #region Disposable编程模式2


        //   private IntPtr handle; // 句柄，属于非托管资源

        //   private Component comp; // 组件，托管资源

        private bool disposed = false;// 是否已释放资源的标志



        //实现 IDisposable接口方法

        //由类的使用者，在外部显式调用，释放类资源，是显式的，手动的

        public void Dispose()

        {

            Dispose(true);// 释放托管和非托管资源



            //将对象从垃圾回收器链表中移除，

            // 从而在垃圾回收器工作时，只释放托管资源，而不执行此对象的析构函数

            GC.SuppressFinalize(this);

        }

        //由垃圾回收器调用，释放非托管资源,该方式是隐式的，自动的

        ~BaseRepository()
        {

            Dispose(false);// 释放非托管资源

        }

        //参数为true表示释放所有资源，只能由使用者调用

        //参数为false表示释放非托管资源，只能由垃圾回收器自动调用

        //如果子类有自己的非托管资源，可以重载这个函数，添加自己的非托管资源的释放

        //但是要记住，重载此函数必须保证调用基类的版本，以保证基类的资源正常释放

        protected virtual void Dispose(bool disposing)

        {

            if (!this.disposed)// 如果资源未释放 这个判断主要用了防止对象被多次释放

            {

                if (disposing)

                {

                    _dbContext.Dispose();// 释放托管资源

                }

                //  closeHandle(handle);// 释放非托管资源

                //handle = IntPtr.Zero;

            }

            this.disposed = true;// 标识此对象已释放

        }



        #endregion
    }

}
