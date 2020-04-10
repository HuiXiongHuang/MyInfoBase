using System;

namespace DataAccessLayer
{

    public partial class MyDBEntities
    {
        public MyDBEntities(String connectString)
            : base(connectString)
        {

        }
    }
}
