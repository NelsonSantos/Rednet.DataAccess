using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rednet.DataAccess
{
    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin
    }

    public enum JoinRelation
    {
        OneToOne,
        OneToMany
    }
    public enum AutomaticValue
    {
        None,
        AutoIncrement,
        BackEndCalculated
    }
    public enum FieldCheck
    {
        None,
        Equal,
        NotEqual,
        Range,
        LessThan,
        GreaterThan,
        Null,
        NotNull,
        NullOrEmpty
    }
    public enum SqlStatementsTypes
    {
        None,
        Select,
        SelectCheck,
        SelectReload,
        Insert,
        Update,
        Delete,
        DeleteAll,
        UnknownStatement
    }

    public enum FireEvent
    {
        None,
        OnBeforeSave,
        OnAfterSave,
        OnBeforeAndAfter
    }
}
