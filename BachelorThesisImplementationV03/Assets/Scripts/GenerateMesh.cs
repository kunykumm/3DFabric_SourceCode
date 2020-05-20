using System;
using UnityEngine;

namespace Dreamteck.Splines
{
    /// <summary>
    /// Extends functionality of GenerateBase class.
    /// Customised for type: Weft-knitting imitations.
    /// Further comments: Suitable for simple continuous lines, where no rotation or position change is needed.
    /// Scenes: Basic Knot.
    /// </summary>
    public class GenerateMesh : GenerateBase
    {
        /// <summary>
        /// Sets up the default net at the start of the scene.
        /// </summary>
        void Start()
        {
            SetupNet(runtimeRows.transform);
            SetupKnotUtility();

            ChangeColumns();
            ChangeRows();

            sizeChanger.ChangeSizesNet();
        }

        /// <summary>
        /// When EditNet button is clicked, this function is called to update all attributes according to knotPrefab.
        /// Deletes all but one row and updates the knotClone. Then the rows and columns are generated again according to 
        /// current number of rows and columns.
        /// </summary>
        public virtual void UpdateNet()
        {
            DeleteRows(prevRows - 1);
            knotClone = runtimeRows.transform.GetChild(0).gameObject;
            knotClone.transform.parent = null;

            SetupNet(runtimeRows.transform);

            prevColumns = 1;
            prevRows = 1;

            ChangeColumns();
            ChangeRows();

            sizeChanger.ChangeSizesNet();
        }

        /// <summary>
        /// In case the value of column and row sliders change, the functions to change the net are called. 
        /// </summary>
        void Update()
        {
            if(updateValues)
            {
                sizeChanger.ChangeSizesNet();
                updateValues = false;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (prevColumns != (int)columns.value) ChangeColumns();
                if (prevRows != (int)rows.value) ChangeRows();
                updateValues = true;
            }
        }

        /// <summary>
        /// Decides how to change columns of the net (increase or decrease the count).
        /// <code>
        ///    int newPoints = diff * (basePointCount - 1);                         / increases the count
        ///    int newCount = currentPointCount + diff * (basePointCount - 1);      / decreases the count
        /// </code>
        /// Wanted number of points in the spline (calculated from difference between 
        /// previous column value and current column value).
        /// </summary>
        protected void ChangeColumns()
        {
            int diff = (int)columns.value - prevColumns;
            if (diff > 0)
            {
                int newPoints = diff * (basePointCount - 1);
                AddColumns(newPoints, splineComputer);
            }
            else
            {
                int newCount = currentPointCount + diff * (basePointCount - 1);
                DeleteColumns(newCount, splineComputer);
            }
            prevColumns = (int)columns.value;
        }

        /// <summary>
        /// Adds new columns.
        /// New points of the spline are added. Their positions are calculated from their twin predecessor in the spline.
        /// </summary>
        /// <param name="newPoints"> Overall count of points wanted in the new spline. </param>
        /// <param name="sc"> SplineComputer of the spline that is being adjusted. </param>
        protected void AddColumns(int newPoints, SplineComputer sc)
        {
            for (int i = 0; i < newPoints; ++i)
            {
                int index = currentPointCount - basePointCount + 1;
                var twinPoint = sc.GetPoint(index);
                sc.SetPointPosition(currentPointCount, new Vector3(twinPoint.position.x + width, twinPoint.position.y, twinPoint.position.z));
                sc.SetPointSize(currentPointCount, point_size);
                sc.SetPointColor(currentPointCount, Color.white);
                sc.SetPointNormal(currentPointCount, sc.GetPointNormal(index));
                currentPointCount++;
            }
        }

        /// <summary>
        /// Deletes columns in the spline.
        /// </summary>
        /// <param name="newCount"> Overall count of points wanted in the new spline. </param>
        /// <param name="sc"> SplineComputer of the spline that is being adjusted. </param>
        protected void DeleteColumns(int newCount, SplineComputer sc)
        {
            if (newCount < basePoints.Length) return;
            SplinePoint[] short_segment = new SplinePoint[newCount];
            SplinePoint[] old_points = sc.GetPoints();
            Array.Copy(old_points, 0, short_segment, 0, newCount);
            sc.SetPoints(short_segment);
            currentPointCount = newCount;
        }

        /// <summary>
        /// Decides how to change rows of the net (increase or decrease the count).
        /// <code>
        ///    int diff = (int)rows.value - prevRows; 
        /// </code>
        /// Calculated the difference between previous row value and current row value.
        /// If the diff is less than 0, the rows are deleted. Otherwise are added.
        /// </summary>
        protected void ChangeRows()
        {
            int diff = (int)rows.value - prevRows;
            if (diff > 0)
            {
                AddRows(diff);
            }
            else
            {
                DeleteRows(-diff);
            }
            prevRows = (int)rows.value;
        }

        /// <summary>
        /// Adds new rows.
        /// The new rows are instantiated from knotClone object in the scene.
        /// The position is calculated and new rows is set to be a child in RuntimeRows.
        /// </summary>
        /// <param name="diff"> Numebr of new rows that should be added. </param>
        protected void AddRows(int diff)
        {
            float curHeight = -(height - heightOffset);

            GameObject newKnot;

            knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.Local;

            for (int i = 0; i < diff; ++i)
            {
                newKnot = Instantiate(knotClone, knotClone.transform.position, Quaternion.identity);
                newKnot.name = "KnotForNet";
                newKnot.tag = "knotrow";
                newKnot.transform.parent = runtimeRows.transform;
                newKnot.transform.position += transform.up * (i + prevRows) * curHeight;

                if ((i + prevRows) % 2 == 1) newKnot.transform.position += transform.right * (width / 2);

                newKnot.GetComponent<SplineComputer>().RebuildImmediate();
            }
            knotClone.GetComponent<SplineComputer>().space = SplineComputer.Space.World;
        }

        /// <summary>
        /// Deletes rows.
        /// </summary>
        /// <param name="diff"> Number of rows to delete (takes the diff rows from bottom). </param>
        protected void DeleteRows(int diff)
        {
            int firstChildDying = prevRows - diff;
            for (int i = 0; i < diff; ++i)
            {
                var child = runtimeRows.transform.GetChild(firstChildDying).gameObject;
                child.transform.parent = null;
                Destroy(child);
            }
        }
    }
}