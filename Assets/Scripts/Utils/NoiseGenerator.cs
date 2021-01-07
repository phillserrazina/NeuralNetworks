using UnityEngine;

namespace Coursework.Utils
{
    public class NoiseGenerator : MonoBehaviour
    {
        /// <summary>
        /// Function Created by Brackeys (https://www.youtube.com/watch?v=bG0uEXV6aHQ)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D GenerateNoise(int width, int height, Vector2 offset, float scale=1f) {
            var texture = new Texture2D(width, height);
            
            for (int x = 0; x < width; x++) 
            {
                for (int y = 0; y < height; y++) 
                {
                    var color = CalculateColor(x, y, width, height, scale, offset);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Color CalculateColor(int x, int y, int width, int height, float scale, Vector2 offset) {
            float xCoord = (float)x / width * scale + offset.x;
            float yCoord = (float)y / height * scale + offset.y;

            float sample = Mathf.PerlinNoise(xCoord, yCoord);
            return new Color(sample, sample, sample);
        }
    }
}
