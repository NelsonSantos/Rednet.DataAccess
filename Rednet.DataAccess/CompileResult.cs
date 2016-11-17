using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Rednet.DataAccess
{
    public class CompileResult
    {
        public string CommandText { get; set; }
        public object Value { get; set; }
        public string ParameterName { get; set; }

        private static object ConvertTo(object obj, Type t)
        {
            Type nut = Nullable.GetUnderlyingType(t);

            if (nut != null)
            {
                if (obj == null) return null;
                return Convert.ChangeType(obj, nut);
            }
            else
            {
                return Convert.ChangeType(obj, t);
            }
        }

        /// <summary>
        /// Compiles a BinaryExpression where one of the parameters is null.
        /// </summary>
        /// <param name="parameter">The non-null parameter</param>
        private static string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter)
        {
            if (expression.NodeType == ExpressionType.Equal)
                return "(" + parameter.CommandText + " is null)";
            else if (expression.NodeType == ExpressionType.NotEqual)
                return "(" + parameter.CommandText + " is not null)";
            else
                throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType.ToString());
        }

        private static string GetOperand(Expression expr)
        {
            var n = expr.NodeType;
            if (n == ExpressionType.GreaterThan)
                return ">";
            else if (n == ExpressionType.GreaterThanOrEqual)
            {
                return ">=";
            }
            else if (n == ExpressionType.LessThan)
            {
                return "<";
            }
            else if (n == ExpressionType.LessThanOrEqual)
            {
                return "<=";
            }
            else if (n == ExpressionType.And)
            {
                return "&";
            }
            else if (n == ExpressionType.AndAlso)
            {
                return "and";
            }
            else if (n == ExpressionType.Or)
            {
                return "|";
            }
            else if (n == ExpressionType.OrElse)
            {
                return "or";
            }
            else if (n == ExpressionType.Equal)
            {
                return "=";
            }
            else if (n == ExpressionType.NotEqual)
            {
                return "!=";
            }
            else
            {
                throw new NotSupportedException("Cannot get SQL for: " + n);
            }
        }
        public static CompileResult CompileExpr(Expression expr, List<string> queryNames, List<object> queryValues, string prefix, string name, bool isLeft = false)
        {
            if (expr == null)
            {
                throw new NotSupportedException("Expression is NULL");
            }
            else if (expr is LambdaExpression)
            {
                return CompileExpr((expr as LambdaExpression).Body, queryNames, queryValues, prefix, name, isLeft);
            }
            else if (expr is BinaryExpression)
            {
                var bin = (BinaryExpression)expr;

                var leftr = CompileExpr(bin.Left, queryNames, queryValues, prefix, name, true);
                var rightr = CompileExpr(bin.Right, queryNames, queryValues, prefix, name, false);

                //If either side is a parameter and is null, then handle the other side specially (for "is null"/"is not null")
                string text;
                if (leftr.CommandText == "?" && leftr.Value == null)
                    text = CompileNullBinaryExpression(bin, rightr);
                else if (rightr.CommandText == "?" && rightr.Value == null)
                    text = CompileNullBinaryExpression(bin, leftr);
                else if (leftr.CommandText != "?" && rightr.CommandText == "?")
                {

                    if (string.IsNullOrEmpty(leftr.ParameterName))
                    {
                        var _split = ("." + leftr.CommandText).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        var _parameter = _split.Aggregate("", (_current, _s) => _current + _s);
                        //text = "(" + leftr.CommandText + " " + GetOperand(bin) + " " + _prefix + leftr.CommandText.Replace(Name + ".", "") + ")";
                        text = "(" + leftr.CommandText + " " + GetOperand(bin) + " " + prefix + _parameter + ")";
                    }
                    else
                        text = "(" + leftr.CommandText + " " + GetOperand(bin) + " " + prefix + leftr.ParameterName + ")";
                }
                else
                    text = "(" + leftr.CommandText + " " + GetOperand(bin) + " " + rightr.CommandText + ")";

                return new CompileResult { CommandText = text };
            }
            else if (expr.NodeType == ExpressionType.Call)
            {

                var call = (MethodCallExpression)expr;
                var args = new CompileResult[call.Arguments.Count];
                var obj = call.Object != null ? CompileExpr(call.Object, queryNames, queryValues, prefix, name, isLeft) : null;

                for (var i = 0; i < args.Length; i++)
                {
                    args[i] = CompileExpr(call.Arguments[i], queryNames, queryValues, prefix, name, isLeft);
                }

                // CHANGE ID = 1 -> comentado ABAIXO pois no momento da definição precisava concatenar com os caracteres "%" no início e no final quando usado like com apenas um parametro
                //                  validar em outras situações
                //                  queryNames.RemoveAt(queryNames.Count - 1);

                var sqlCall = "";
                var _parName = "";

                if (call.Method.Name == "Like" && args.Length == 2)
                {
                    sqlCall = "(" + args[0].CommandText + " like " + args[1].CommandText + ")";
                }
                else if (call.Method.Name == "Contains" && args.Length >= 2)
                {
                    //sqlCall = "(" + args[1].CommandText + " in " + args[0].CommandText + ")";
                    sqlCall = string.Format("({0}.{1} in {2})", name, args[1].CommandText, args[0].CommandText);
                }
                else if (call.Method.Name == "Contains" && args.Length == 1)
                {
                    if (call.Object != null && call.Object.Type == typeof(string))
                    {
                        // CHANGE ID = 1
                        sqlCall = string.Format("({0}.{1} like {2}{1})", name, obj.CommandText, prefix);
                        queryValues[0] = "%" + queryValues[0] + "%";
                    }
                    else
                    {
                        // CHANGE ID = 1
                        sqlCall = string.Format("({0}.{1} in ({2}{1}))", name, obj.CommandText, prefix);
                    }
                }
                else if (call.Method.Name == "StartsWith" && args.Length == 1)
                {
                    // CHANGE ID = 1
                    sqlCall = string.Format("({0}.{1} like {2}{1})", name, obj.CommandText, prefix);
                    queryValues[0] = queryValues[0] + "%";
                }
                else if (call.Method.Name == "EndsWith" && args.Length == 1)
                {
                    // CHANGE ID = 1
                    sqlCall = string.Format("({0}.{1} like {2}{1})", name, obj.CommandText, prefix);
                    queryValues[0] = "%" + queryValues[0];
                }
                else if (call.Method.Name == "Equals" && args.Length == 1)
                {
                    sqlCall = "(" + obj.CommandText + " = (" + args[0].CommandText + "))";
                }
                else if (call.Method.Name == "ToLower")
                {
                    sqlCall = "(lower(" + obj.CommandText + "))";
                }
                else if (call.Method.Name == "ToUpper")
                {
                    sqlCall = "(upper(" + obj.CommandText + "))";
                }
                else if (call.Method.Name == "Trim")
                {
                    sqlCall = "(ltrim(rtrim(" + obj.CommandText + ")))";
                    _parName = obj.CommandText;
                }
                else
                {
                    sqlCall = call.Method.Name.ToLower() + "(" + string.Join(",", args.Select(a => a.CommandText).ToArray()) + ")";
                }
                return new CompileResult { CommandText = sqlCall, ParameterName = _parName };

            }
            else if (expr.NodeType == ExpressionType.Constant)
            {
                var _c = (ConstantExpression)expr;
                queryValues.Add(_c.Value);
                return new CompileResult
                {
                    CommandText = "?",
                    Value = _c.Value
                };
            }
            else if (expr.NodeType == ExpressionType.Convert)
            {
                var u = (UnaryExpression)expr;
                var ty = u.Type;
                var valr = CompileExpr(u.Operand, queryNames, queryValues, prefix, name, isLeft);
                return new CompileResult
                {
                    CommandText = valr.CommandText,
                    Value = valr.Value != null ? ConvertTo(valr.Value, ty) : null
                };
            }
            //else if (expr is ParameterExpression)
            //{
            //    var _mem = (ParameterExpression)expr;
            //    var _a = _mem;
            //}
            //else if (expr.NodeType == ExpressionType.Parameter)
            //{
            //    var r = CompileExpr((expr as ParameterExpression), queryNames, queryValues, isLeft);
            //    return new CompileResult { CommandText = r.ParameterName };
            //}
            else if (expr.NodeType == ExpressionType.MemberAccess)
            {

                var _mem = (MemberExpression)expr;
                var _joinAtt = _mem.Member.GetCustomAttributes(typeof(JoinFieldAttribute));
                CompileResult _rr = null;

                if (_mem.Expression != null && _mem.Expression.NodeType == ExpressionType.MemberAccess && !_joinAtt.Any())
                {
                    _rr = CompileExpr(_mem.Expression, queryNames, queryValues, prefix, name, isLeft);
                }

                if (_mem.Expression != null && _mem.Expression.NodeType == ExpressionType.Parameter)
                {
                    //
                    // This is a column of our table, output just the column name
                    // Need to translate it if that column name is mapped
                    //

                    if (_joinAtt.Any())
                    {
                        //if (_mem.Expression is ParameterExpression)
                        //{
                        //    var r = CompileExpr(_mem.Expression, queryNames, queryValues, isLeft);
                        //}
                        var _c = (isLeft ? _mem.Type.Name + "." : "");// + ((PropertyInfo)_mem.Member).Name;
                        return new CompileResult { CommandText = _c, Value = "" };
                    }

                    var _columnName = (isLeft ? string.Format("{0}.", name) : "") + _mem.Member.Name;
                    queryNames.Add(_columnName.Replace(".", ""));
                    return new CompileResult { CommandText = _columnName };
                }
                else
                {
                    object obj = null;
                    if (_mem.Expression != null)
                    {
                        var r = CompileExpr(_mem.Expression, queryNames, queryValues, prefix, name, isLeft);
                        if (r.Value == null)
                        {
                            throw new NotSupportedException("Member access failed to compile expression");
                        }
                        if (r.CommandText == "?")
                        {
                            queryValues.RemoveAt(queryValues.Count - 1);
                        }
                        obj = r.Value;
                    }

                    //
                    // Get the member value
                    //
                    object val = null;

                    if (_mem.Member is PropertyInfo)
                    {
                        if (_rr != null)
                        {
                            var _pp = (_mem.Member as PropertyInfo);
                            if (!_rr.CommandText.Equals("?"))
                            {
                                _rr.CommandText += _pp.Name;
                                _rr.Value = Activator.CreateInstance(_pp.PropertyType);
                                queryNames.Add(_rr.CommandText.Replace(".", ""));
                            }
                        }
                        else
                        {
                            var m = (PropertyInfo)_mem.Member;
                            val = m.GetValue(obj, null);
                        }
                    }
                    else if (_mem.Member is FieldInfo)
                    {
                        var m = (FieldInfo)_mem.Member;
                        val = m.GetValue(obj);
                    }
                    else
                    {
                        throw new NotSupportedException("MemberExpr: " + _mem.Member.DeclaringType);
                    }

                    //
                    // Work special magic for enumerables
                    //
                    if (val != null && val is System.Collections.IEnumerable && !(val is string) && !(val is System.Collections.Generic.IEnumerable<byte>))
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.Append("(");
                        var head = "";
                        var _count = 0;
                        foreach (var a in (System.Collections.IEnumerable)val)
                        {
                            _count++;
                            queryValues.Add(a);
                            queryNames.Add(string.Format("{0}{1}{2}", name, _mem.Member.Name, _count));
                            sb.Append(head);
                            //sb.Append("?");
                            sb.AppendFormat("{0}{1}{2}{3}", prefix, name, _mem.Member.Name, _count);
                            head = ",";
                        }
                        sb.Append(")");
                        return new CompileResult
                        {
                            CommandText = sb.ToString(),
                            //CommandText = string.Format("{0}{1}{2}", prefix, name, _mem.Member.Name),
                            Value = val
                        };
                    }
                    else
                    {

                        if (_rr != null) return _rr;

                        queryValues.Add(val);
                        return new CompileResult
                        {
                            CommandText = "?",
                            Value = val
                        };
                    }
                }
            }
            throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
        }

    }
}