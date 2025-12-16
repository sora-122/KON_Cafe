# KONかふぇ (Prototype)

![Unity](https://img.shields.io/badge/Unity-6000.0.x-black.svg?style=flat&logo=unity)
![License](https://img.shields.io/badge/License-Proprietary-red.svg)

## 📖 プロジェクト概要
『KONかふぇ』は、癒やしと経営戦略を融合させたカフェ経営シミュレーションゲームです。
個人制作のポートフォリオ作品として開発しており、**「保守性の高い設計（SRE視点）」**と**「拡張性のあるアーキテクチャ」**を重視して実装しています。

> **Note**
> 本リポジトリはプロトタイプ版です。有料アセット等は `.gitignore` により除外されているため、Cloneしてそのまま実行することはできません（コード閲覧用としての公開です）。

## 🎮 技術スタック (Tech Stack)
本プロジェクトでは、実務でのチーム開発や大規模運用を見据え、以下の技術・設計パターンを採用しています。

### Architecture & Design Pattern
* **MVP (Model-View-Presenter):** プレゼンテーション層とロジック層を分離し、テスタビリティを確保。
* **VContainer (Dependency Injection):** 依存関係の注入により、疎結合な設計を実現。
* **UniTask:** 非同期処理の最適化と、メモリリーク防止（CancellationTokenの徹底）。

### Quality Assurance & Dev Tools (SRE視点)
* **Automatic Data Validation:** `OnValidate`を活用し、データ変更時（アイコン設定等）に不整合があれば即座にログを出力する自動チェック機構を実装。
* **Debug Tools:** 開発サイクルを高速化するため、セーブデータのリセット機能などをエディタ拡張として独自実装。
* **Assembly Definition (.asmdef):** コンパイル時間の短縮と依存関係の整理、テストコードの分離。

## 📂 ディレクトリ構成 (抜粋)
主要な自作コードは `Assets/_Project` 配下に集約しています。

```text
Assets/_Project/
├── Features/           # 機能単位での分割 (画面やゲーム機能ごと)
│   ├── Cafe/          # カフェパートのMVP
│   ├── Home/          # ホーム画面のMVP
│   └── Management/    # 経営パートのMVP
├── Core/               # 共通基盤 (Manager, Utility)
│   ├── Data/          # マスターデータ (ScriptableObject)
└── Tests/              # テストコード (EditMode / PlayMode)