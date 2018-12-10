using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float forwardSpeed = 100f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 150.0f;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * 150.0f * -1;
        float move = Input.GetAxis("Vertical") * Time.deltaTime * forwardSpeed;

        transform.Rotate(mouseY, mouseX, 0);

        /* Set z component of rotation transform back to 0 */
        Quaternion tempQ = transform.rotation;  
		tempQ.eulerAngles = new Vector3(tempQ.eulerAngles.x, tempQ.eulerAngles.y, 0);
		transform.rotation = tempQ;

        transform.Translate(0, 0, move);
    }
}