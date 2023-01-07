using System.Collections;

namespace Ieedo
{
    public class UIAssessmentHeader : UIScreen
    {
        public override ScreenID ID => ScreenID.AssessmentHeader;

        public UITextContent Title;

        public IEnumerator ShowCategory(CategoryDefinition category)
        {
            Title.Text.Key = category.Title.Key;
            Title.BG.color = category.Color;

            if (!IsOpen)
                yield return OpenCO();
        }
    }
}
