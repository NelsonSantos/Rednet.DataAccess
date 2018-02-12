#if !PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rednet.DataAccess.FastMember
{
    /// <summary>
    /// Represents an abstracted view of the members defined for a type
    /// </summary>
    public sealed class MemberSet : IEnumerable<Member>, IList<Member>
    {
        Member[] members;
        internal MemberSet(Type type)
        {
#if WINDOWS_PHONE_APP
            members = type.GetRuntimeProperties().Cast<MemberInfo>().Concat(type.GetRuntimeFields().Cast<MemberInfo>()).OrderBy(x => x.Name).Select(member => new Member(member)).ToArray();
#else
            members = type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields().Cast<MemberInfo>()).OrderBy(x => x.Name, StringComparer.InvariantCulture).Select(member => new Member(member)).ToArray();
#endif
        }
        /// <summary>
        /// Return a sequence of all defined members
        /// </summary>
        public IEnumerator<Member> GetEnumerator()
        {
            foreach (var member in members) yield return member;
        }
        /// <summary>
        /// Get a member by index
        /// </summary>
        public Member this[int index]
        {
            get { return members[index]; }
        }
        /// <summary>
        /// The number of members defined for this type
        /// </summary>
        public int Count { get { return members.Length; } }
        Member IList<Member>.this[int index]
        {
            get { return members[index]; }
            set { throw new NotSupportedException(); }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        bool ICollection<Member>.Remove(Member item) { throw new NotSupportedException(); }
        void ICollection<Member>.Add(Member item) { throw new NotSupportedException(); }
        void ICollection<Member>.Clear() { throw new NotSupportedException(); }
        void IList<Member>.RemoveAt(int index) { throw new NotSupportedException(); }
        void IList<Member>.Insert(int index, Member item) { throw new NotSupportedException(); }

        bool ICollection<Member>.Contains(Member item) { return members.Contains(item); }
        void ICollection<Member>.CopyTo(Member[] array, int arrayIndex) { members.CopyTo(array, arrayIndex); }
        bool ICollection<Member>.IsReadOnly { get { return true; } }
        int IList<Member>.IndexOf(Member member) { return Array.IndexOf<Member>(members, member); }
        
    }
    /// <summary>
    /// Represents an abstracted view of an individual member defined for a type
    /// </summary>
    public sealed class Member
    {
        private readonly MemberInfo member;
        private object m_CanWrite = null;
        private Type m_Type = null;

        internal Member(MemberInfo member)
        {
            this.member = member;
        }
        /// <summary>
        /// The name of this member
        /// </summary>
        public string Name { get { return member.Name; } }
        /// <summary>
        /// The type of value stored in this member
        /// </summary>
        public Type Type
        {
            get
            {
                if (m_Type != null) return m_Type;
#if WINDOWS_PHONE_APP
                m_Type = ((PropertyInfo)this.member).PropertyType;
#else
                switch (member.MemberType)
                {
                    case MemberTypes.Field: 
                        m_Type = ((FieldInfo)member).FieldType;
                        break;
                    case MemberTypes.Property: 
                        m_Type = ((PropertyInfo)member).PropertyType;
                        break;
                    default: 
                        throw new NotSupportedException(member.MemberType.ToString());
                }
#endif
                return m_Type;
            }
        }

        /// <summary>
        /// Indicates when this member can be writed
        /// </summary>
        public bool CanWrite
        {
            get
            {
                if (m_CanWrite != null) return (bool) m_CanWrite;

#if WINDOWS_PHONE_APP
                try
                {
                    m_CanWrite = member.DeclaringType.GetRuntimeProperty(member.Name).CanWrite;
                }
                catch (NullReferenceException nrex)
                {
                    m_CanWrite = false;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
#else
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        m_CanWrite =  true;
                        break;
                    case MemberTypes.Property: 
                        m_CanWrite = ((PropertyInfo)member).CanWrite;
                        break;
                    default: 
                        throw new NotSupportedException(member.MemberType.ToString());
                }
#endif
                return (bool) m_CanWrite;
            }
        }

        /// <summary>
        /// Is the attribute specified defined on this type
        /// </summary>
        public bool IsDefined(Type attributeType)
        {
#if WINDOWS_PHONE_APP
            return false;
#else
            if (attributeType == null) throw new ArgumentNullException("attributeType");
            return Attribute.IsDefined(member, attributeType);
#endif
        }


    }
}
#endif
