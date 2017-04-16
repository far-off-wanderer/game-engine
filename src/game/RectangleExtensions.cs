namespace Game
{
    using Microsoft.Xna.Framework;

    static class RectangleExtensions
    {
        public static Rectangle Fill(this Rectangle target, int width, int height)
        {
            var target_aspect = (float)target.Width / target.Height;
            var source_aspect = (float)width / height;

            if (target_aspect < source_aspect)
            {
                var scale = (float)target.Height / height;
                var new_width = (int)(scale * width);
                return new Rectangle(
                    -(new_width - target.Width) / 2,
                    0,
                    new_width,
                    target.Height
                );
            }
            else
            {
                var scale = (float)target.Width / width;
                var new_height = (int)(scale * height);
                return new Rectangle(
                    0,
                    -(new_height - target.Height) / 2,
                    target.Width,
                    new_height
                );
            }
        }

        public static Rectangle FillHorizontally(this Rectangle target, int width, int height)
        {
            var target_aspect = (float)target.Width / target.Height;
            var source_aspect = (float)width / height;

            var scale = (float)target.Width / width;
            var new_height = (int)(scale * height);
            return new Rectangle(
                0,
                -(new_height - target.Height) / 2,
                target.Width,
                new_height
            );
        }
    }

    static class SizeExtension
    {
        public static Rectangle Fill(this (int Width, int Height) target, int width, int height)
        {
            var target_aspect = (float)target.Width / target.Height;
            var source_aspect = (float)width / height;

            if (target_aspect < source_aspect)
            {
                var scale = (float)target.Height / height;
                var new_width = (int)(scale * width);
                return new Rectangle(
                    -(new_width - target.Width) / 2,
                    0,
                    new_width,
                    target.Height
                );
            }
            else
            {
                var scale = (float)target.Width / width;
                var new_height = (int)(scale * height);
                return new Rectangle(
                    0,
                    -(new_height - target.Height) / 2,
                    target.Width,
                    new_height
                );
            }
        }

        public static Rectangle FillHorizontally(this (int Width, int Height) target, int width, int height)
        {
            var target_aspect = (float)target.Width / target.Height;
            var source_aspect = (float)width / height;

            var scale = (float)target.Width / width;
            var new_height = (int)(scale * height);
            return new Rectangle(
                0,
                -(new_height - target.Height) / 2,
                target.Width,
                new_height
            );
        }
    }
}
