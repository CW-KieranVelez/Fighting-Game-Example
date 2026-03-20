using System.Text;
using UnityEngine;

namespace FightTest.Input
{
    public class InputBufferDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerInputProvider _provider;
        [SerializeField] private Vector2 _position = new Vector2(10, 10);

        private void OnGUI()
        {
            if (_provider == null)
            {
                return;
            }

            var (light, heavy) = _provider.GetBufferSnapshot();

            var style = new GUIStyle(GUI.skin.box) { fontSize = 14, alignment = TextAnchor.UpperLeft };
            style.normal.textColor = Color.white;

            var sb = new StringBuilder();
            sb.AppendLine("Input Buffer:");
            sb.AppendLine($"  Light : {(light > 0 ? $"{light} tick(s)" : "—")}");
            sb.AppendLine($"  Heavy : {(heavy > 0 ? $"{heavy} tick(s)" : "—")}");
            sb.AppendLine($"  Throw : {(light > 0 && heavy > 0 ? "YES" : "—")}");

            var content = sb.ToString();
            var size = style.CalcSize(new GUIContent(content));
            GUI.Box(new Rect(_position.x, _position.y, size.x + 16, size.y + 8), content, style);
        }
    }
}