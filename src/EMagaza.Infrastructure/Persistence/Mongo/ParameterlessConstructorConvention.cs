using System.Reflection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace EMagaza.Infrastructure.Persistence.Mongo;

/// <summary>
/// Entity'ler veritabanından her zaman parametresiz (private) constructor + alan atamasıyla okunur.
/// Aksi halde sürücü doğrulamalı public constructor'ı seçer ve belgede eksik bir alan varsa okuma hata verir.
/// </summary>
public class ParameterlessConstructorConvention : ConventionBase, IClassMapConvention
{
    public void Apply(BsonClassMap classMap)
    {
        if (classMap.ClassType.IsAbstract) return;   // Entity gibi soyut taban sınıflar oluşturulamaz

        var constructor = classMap.ClassType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, binder: null, Type.EmptyTypes, modifiers: null);

        if (constructor is not null) classMap.MapConstructor(constructor);
    }
}
