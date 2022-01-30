using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Helpers
{
    public static class InOrderTraversalHelper
    {
        public static IEnumerable<T> PreOrder<T>(IEnumerable<T?> source, Func<T, IEnumerable<T>> childSelector)
        {
            Stack<IEnumerator<T?>> stack = new Stack<IEnumerator<T?>>();
            if (source is not null)
            {
                stack.Push(source.GetEnumerator());
            }
            else
            {
                throw new NullReferenceException();
            }

            try
            {
                while (stack.Any())
                {
                    IEnumerator<T?> topElement = stack.Pop();
                    if (topElement.MoveNext())
                    {
                        stack.Push(topElement);
                        if (topElement.Current is not null)
                        {
                            IEnumerable<T> children = childSelector(topElement.Current);
                            stack.Push(children.GetEnumerator());
                            yield return topElement.Current;
                        }
                    }
                    else
                    {
                        topElement.Dispose();
                    }
                }
            }
            finally
            {
                foreach (IEnumerator<T?> iterator in stack)
                {
                    iterator.Dispose();
                }
            }
        }
    }
}
