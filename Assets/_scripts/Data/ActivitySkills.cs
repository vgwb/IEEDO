using System;

namespace Ieedo
{
    public enum ActivityCategory
    {
        None = 0,
        category_attention = 1,
        category_emotions = 2,
        category_flexibility = 3,
        category_math = 4,
        category_memory = 5,
        category_problem_solving = 6,
        category_speed = 7,
        category_word = 8,
    }

    [Flags]
    public enum ActivitySkills
    {
        None = 0,
        skill_emotions_sense = 1 << 0,
        skill_emotions_inclusiveness = 1 << 2,
        skill_attention_divided = 1 << 3,
        skill_attention_selective = 1 << 4,
        skill_attention_task_switching = 1 << 5,
        skill_coordination_timing = 1 << 6,
        skill_field_of_view = 1 << 7,
        skill_logical_reasoning = 1 << 8,
        skill_math_numerical_calculation = 1 << 9,
        skill_math_numerical_estimation = 1 << 10,
        skill_math_probabilistic_reasoning = 1 << 11,
        skill_memory_working = 1 << 12,
        skill_planning = 1 << 13,
        skill_processing_speed = 1 << 14,
        skill_proportional_reasoning = 1 << 15,
        skill_reading_comprehension = 1 << 16,
        skill_response_inhibition = 1 << 17,
        skill_spatial_orientation = 1 << 18,
        skill_spatial_reasoning = 1 << 19,
        skill_spatial_recall = 1 << 20,
        skill_visualization = 1 << 21,
        skill_vocabulary_proficiency = 1 << 22,
        skill_word_verbal_fluency = 1 << 23,
    }
}
