from datasets import load_dataset
import argparse
import json
import pandas as pd
from PIL.Image import Image as PngImageFile


def serialize_example(example):
    """Remove non-serializable fields (like images) from an example"""
    return {k: v for k, v in example.items() if not isinstance(v, PngImageFile)}


def download_and_save_dataset(dataset_name, save_format="json", output_dir="."):
    """
    Downloads a Hugging Face dataset and saves it while ignoring image fields.
    """
    print(f"Loading dataset: {dataset_name}")
    dataset = load_dataset(dataset_name)

    print(f"Saving dataset to {output_dir} as {save_format}...")
    for split in dataset.keys():
        filename = f"{dataset_name.replace('/', '_')}_{split}.{save_format}"
        filepath = f"{output_dir}/{filename}"

        if save_format == "json":
            with open(filepath, "w", encoding="utf-8") as f:
                for example in dataset[split]:
                    try:
                        serialized = serialize_example(example)
                        json.dump(serialized, f, ensure_ascii=False)
                        f.write("\n")  # Newline-delimited JSON
                    except Exception as e:
                        print(f"Skipping example due to error: {str(e)}")
                        continue

        elif save_format == "csv":
            # First convert to list of serialized examples
            serialized_examples = [serialize_example(ex) for ex in dataset[split]]
            # Then convert to DataFrame
            df = pd.DataFrame(serialized_examples)
            # Save to CSV
            df.to_csv(filepath, encoding="utf-8", index=False)

        elif save_format == "arrow":
            dataset[split] = dataset[split].map(serialize_example)
            dataset[split].save_to_disk(filepath)
        else:
            raise ValueError(f"Unsupported format: {save_format}")

        print(f"Saved {split} split to: {filepath}")
    print("Done!")


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    # parser.add_argument("--dataset", default="lmms-lab/OCRBench-v2")
    # parser.add_argument("--dataset", default="ajeshmahto/kyc")
    parser.add_argument("--dataset", default="Johnmahith/employee_attrition")
    parser.add_argument("--format", default="json", choices=["json", "csv", "arrow"])
    parser.add_argument("--output", default=".")
    args = parser.parse_args()

    download_and_save_dataset(args.dataset, args.format, args.output)