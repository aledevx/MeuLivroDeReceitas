using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRecipeBook.Domain.Extensions;
public static class StringExtension
{
    public static bool NotEmpty([NotNullWhen(true)] this string? value) => String.IsNullOrWhiteSpace(value).IsFalse();
}
