using System;
namespace ActivityInfo.Query
{
    public interface IColumnView
    {
        object this[int rowIndex] { get; }

        string GetString(int rowIndex);

        double GetDouble(int rowIndex);
    }

    public class EmptyColumnView : IColumnView {
        private ColumnType type;
        private int numRows;

        public EmptyColumnView(ColumnType type, int numRows)
        {
            this.type = type;
            this.numRows = numRows;
        }

        public object this[int rowIndex] => null;

        public double GetDouble(int rowIndex)
        {
            return Double.NaN;
        }

        public string GetString(int rowIndex)
        {
            return null;
        }
    }

    public class StringArrayColumnView : IColumnView {
        private string[] values;

        public StringArrayColumnView(string[] values)
        {
            this.values = values;
        }

        public object this[int rowIndex] => values[rowIndex];

        public double GetDouble(int rowIndex)
        {
            return Double.NaN;
        }

        public string GetString(int rowIndex)
        {
            return this.values[rowIndex];
        }
    }

    public class DoubleArrayColumnView : IColumnView {

        private double[] values;

        public DoubleArrayColumnView(double[] values) {
            this.values = values;
        }

        public object this[int rowIndex] {
            get
            {
                double value = values[rowIndex];
                if(Double.IsNaN(value)) {
                    return null;
                } else {
                    return value;
                }
            }
        }

        public double GetDouble(int rowIndex)
        {
            return values[rowIndex];
        }

        public string GetString(int rowIndex)
        {
            double value = values[rowIndex];
            if(Double.IsNaN(value)) {
                return null;
            }
            return value.ToString();
        }
    }

    public class BooleanArrayColumnView : IColumnView {

        public static readonly byte MISSING = 0xFF;

        private byte[] values;

        public BooleanArrayColumnView(byte[] values)
        {
            this.values = values;
        }

        public object this[int rowIndex] {
            get
            {
                byte value = values[rowIndex];
                if (value == MISSING)
                {
                    return null;
                } else if(value == 0) {
                    return false;
                } else {
                    return true;
                }
            }
        }

        public double GetDouble(int rowIndex)
        {
            byte value = values[rowIndex];
            if(value == MISSING) {
                return Double.NaN;
            } else {
                return (value != 0) ? 1 : 0;
            }
        }

        public string GetString(int rowIndex)
        {
            byte value = values[rowIndex];
            if (value == MISSING)
            {
                return null;
            }
            else
            {
                return (value != 0) ? "true" : "false";
            }
        }
    }

    public class ConstantColumnView : IColumnView {
        private int numRows;
        private ColumnType type;
        private object value;

        public ConstantColumnView(ColumnType type, object value, int numRows)
        {
            this.type = type;
            this.value = value;
            this.numRows = numRows;
        }

        public object this[int rowIndex] => value;

        public double GetDouble(int rowIndex)
        {
            if(value is double) {
                return (double)value;
            } else {
                return Double.NaN;
            }
        }

        public string GetString(int rowIndex)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
