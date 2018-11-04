using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace euler_graph_generator.GraphMethods
{
    public class MatrixMethod
    {

        public double[][] Matrix { get; private set; }
        public DataTable DataTable {get; private set;}

        private int _numberOfVertices { get; set; }
        private double _probabilityValue { get; set; }

        public MatrixMethod(int numberOfVertices, double probabilityValue)
        {

            _numberOfVertices = numberOfVertices;
            _probabilityValue = probabilityValue;

            DataTable = new DataTable();

            Matrix = new double[numberOfVertices][];
            for (int i = 0; i < _numberOfVertices; i++)
            {
                Matrix[i] = new double[_numberOfVertices + 1];
            }
            
            SetMatrixColumns();
            FillTheMatrix();
            FillTheSecondHalfOfTheMatrix();
            CalculateMatrixSum();
            FillDataTable();
        }

        private void SetMatrixColumns()
        {
            for (int i = 0; i < _numberOfVertices + 1; i++)
            {
                if (i == _numberOfVertices)
                {
                    DataTable.Columns.Add(new DataColumn("Suma"));
                }
                else
                {
                    DataTable.Columns.Add(new DataColumn((i + 1).ToString()));
                }
            }
        }

        private void FillTheMatrix()
        {
            int j = 0;
            double ArraySingleValue = 0;
            Random random = new Random();
            for (int i = 0; i < _numberOfVertices; i++)
            {
                j = i;
                while (j < _numberOfVertices)
                {
                    ArraySingleValue = random.NextDouble();
                    if (i != j)
                    {
                        if (ArraySingleValue <= _probabilityValue)
                        {
                            ArraySingleValue = 1;

                        }
                        else
                        {
                            ArraySingleValue = 0;
                        }
                    }
                    else
                    {
                        ArraySingleValue = 0;
                    }
                    Matrix[i][j] = ArraySingleValue;
                    j++;
                }

            }
        }

        private void FillTheSecondHalfOfTheMatrix()
        {
            for (int i = 0; i < _numberOfVertices; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    Matrix[i][j] = Matrix[j][i];
                }
            }
        }

        private void CalculateMatrixSum()
        {
            for (int i = 0; i < _numberOfVertices; i++)
            {
                double sum = 0;
                for (int j = 0; j < _numberOfVertices; j++)
                {
                    sum += Matrix[i][j];
                }
                Matrix[i][_numberOfVertices] = sum;
            }
        }

        private void FillDataTable()
        {
            for (int i = 0; i < _numberOfVertices; i++)
            {
                var newRow = DataTable.NewRow();

                for (int j = 0; j <= _numberOfVertices; j++)
                {
                    newRow[j] = Matrix[i][j];
                }

                DataTable.Rows.Add(newRow);
            }
        }


    }
}
