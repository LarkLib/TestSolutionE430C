using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LarkLib.Common.Utilities
{
    public class Util
    {
        #region Enum

        public static T ParseEnumValue<T, TU>(TU value) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Argument 'T' is not a enum");
            }
            if (typeof(TU).IsClass && value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentException("value is not a valid enum value");
            }
            return (T)Enum.Parse(enumType: typeof(T), value: value.ToString(), ignoreCase: true);
        }

        #endregion

        public static T CastClase<S, T>(S source)
        {
            if (source == null)
            {
                return default(T);
            }
            Type sourceType = source.GetType();
            Type target = typeof(T);
            var targetInstance = Activator.CreateInstance(target, false);
            if (targetInstance == null)
            {
                return default(T);
            }
            var sourcePropertyMembers = from sourceProperty in sourceType.GetMembers().ToList()
                                        where sourceProperty.MemberType == MemberTypes.Property
                                        select sourceProperty;
            if (sourcePropertyMembers == null || !sourcePropertyMembers.Any())
            {
                return default(T);
            }
            var targetePropertyMembers = from sourceProperty in target.GetMembers().ToList()
                                         where sourceProperty.MemberType == MemberTypes.Property
                                         select sourceProperty;
            if (targetePropertyMembers == null || !targetePropertyMembers.Any())
            {
                return default(T);
            }
            List<MemberInfo> members = targetePropertyMembers.Where(memberInfo => targetePropertyMembers.Select(c => c.Name)
               .ToList().Contains(memberInfo.Name)).ToList();
            if (members == null || !members.Any())
            {
                return default(T);
            }
            PropertyInfo propertyInfo;
            object value;
            foreach (var memberInfo in members)
            {
                propertyInfo = typeof(T).GetProperties().Where(info => info.Name == memberInfo.Name).LastOrDefault();

                var sourceProperty = source.GetType().GetProperty(memberInfo.Name);
                if (sourceProperty != null)
                {
                    value = sourceProperty.GetValue(source, null);
                    propertyInfo.SetValue(targetInstance, value, null);
                }
            }
            return (T)targetInstance;
        }

        public static DateTime GetNextRunTime(int intervalMinutes, int offsetMillisecond)
        {
            var nextRunTime = DateTime.Now.AddMinutes(intervalMinutes);
            if (nextRunTime.Second >= 30)
            {
                return nextRunTime.AddSeconds(59 - nextRunTime.Second).AddMilliseconds(1000 - nextRunTime.Millisecond + offsetMillisecond);
            }
            else
            {
                return nextRunTime.AddSeconds(-nextRunTime.Second).AddMilliseconds(-nextRunTime.Millisecond + offsetMillisecond);
            }
        }
    }
}
