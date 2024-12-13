using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IRepository : IDisposable
    {
        // Clases genericas para realizar las operaciones del CRUD en todas nuestras entidades
        // En este caso de Productos y Categorias

        // Crear un nuevo registro
        TEntity Create<TEntity>(TEntity toCreate) where TEntity : class;
        // Eliminar un registro
        bool Delete<TEntity>(TEntity toDelete) where TEntity : class;
        // Actualizar un registro
        bool Update<TEntity>(TEntity toUpdate) where TEntity : class;
        // Listar un solo registro segun el criterio o filtro
        TEntity Retrieve<TEntity>(Expression<Func<TEntity, bool>> criteria)
            where TEntity : class;
        // Listar todos los registros segun una condicion o filtro
        List<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> criteria)
            where TEntity : class;
        // Listar todos los registros
        List<TEntity> RetrieveAll<TEntity>() where TEntity : class;
    }
}
