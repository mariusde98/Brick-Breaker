using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Getting the minMax Attribute
        MinMaxSliderAttribute minMaxAttribute = (MinMaxSliderAttribute)attribute;

        //PrefixLabel returns the rect of the right part, without the label part
        Rect controlRect = EditorGUI.PrefixLabel(position, label);

        int space = 5; //Space between the rects
        Rect left = new Rect(controlRect.x, controlRect.y, controlRect.width / 8 - space, controlRect.height); //Rectangle for the first float value
        Rect right = new Rect(controlRect.x + controlRect.width - left.width, controlRect.y, left.width, controlRect.height);//Rectangle for the second float value
        Rect mid = new Rect(left.xMax + space, controlRect.y, right.x - (left.xMax + space) - space, controlRect.height); //Rectangle for the slider

        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            //Start checking if values got changed
            EditorGUI.BeginChangeCheck();

            //Vector of the values
            Vector2 vector = property.vector2Value;

            float minVal = vector.x;
            float maxVal = vector.y;

            //F2 limits the float to two decimal places
            minVal = EditorGUI.FloatField(left, float.Parse(minVal.ToString("F2")));
            maxVal = EditorGUI.FloatField(right, float.Parse(maxVal.ToString("F2")));

            //Creating the Min Max Slider
            EditorGUI.MinMaxSlider(mid, ref minVal, ref maxVal,
            minMaxAttribute.min, minMaxAttribute.max);

            //Clamping the values between the range
            if (minVal < minMaxAttribute.min)
            {
                minVal = minMaxAttribute.min;
            }

            if (maxVal > minMaxAttribute.max)
            {
                maxVal = minMaxAttribute.max;
            }

            //Swapping min and max if min>max
            if (minVal > maxVal)
            {
                float tmp = minVal;
                minVal = maxVal;
                maxVal = tmp;
            }

            //If the change is completed return the new vector2
            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = new Vector2(minVal, maxVal);
            }

        }
    }
}