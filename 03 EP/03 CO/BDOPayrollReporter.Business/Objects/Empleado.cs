using System;
using System.Diagnostics;

namespace BDOPayrollReporter.Business.Objects
{
    /// <summary>
    /// Representa un empleado.
    /// </summary>
    [
        DebuggerDisplay("Empleado: {Id, nq} {Nombre, nq}")
    ]
    public class Empleado
    {
        #region Fields

        private string id = String.Empty;
        private string nombre = String.Empty;
        private string paterno = String.Empty;
        private string materno = String.Empty;
        private string email = String.Empty;

        #endregion Fields

        #region Properties

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public string NombreCompleto
        {
            get { return string.Format("{0} {1} {2}", paterno, materno, nombre); }
        }

        public string Paterno
        {
            get { return paterno; }
            set { paterno = value; }
        }

        public string Materno
        {
            get { return materno; }
            set { materno = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        #endregion Properties
    }
}