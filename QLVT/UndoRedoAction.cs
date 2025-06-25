using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLVT
{
    public class UndoRedoAction
    {
        public string DoSql { get; }
        public string UndoSql { get; }
        public int OriginalPosition { get; }
        public object AffectedKey { get; }

        public UndoRedoAction(string doSql, string undoSql, int originalPosition, object affectedKey)
        {
            DoSql = doSql;
            UndoSql = undoSql;
            OriginalPosition = originalPosition;
            AffectedKey = affectedKey;
        }
    }
}
