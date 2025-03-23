# supLauncher-CS

Windows用メニューランチャーアプリケーション「supLauncher」のC#版です。

## 概要

このアプリケーションは、設定したアプリケーションやファイルをカスタマイズしたボタンから簡単に起動できるランチャーメニューを提供します。VB.NET版のsupLauncherをC#に完全に移植したものです。

## 開発環境

- .NET Framework 4.8
- Visual Studio 2019以上推奨
- Windows 7以上（Windows 10/11で動作確認済み）

## 機能

- カスタマイズ可能なボタンメニュー
- 複数のアプリケーションの実行
- メニュー間の連携と階層化
- XML形式でのメニュー定義ファイル管理
- カスタマイズ可能な外観設定
  - フォント・色・サイズなどの変更
  - 背景画像設定
- パスワードによる編集保護

## ファイル構成

- `Program.cs` - アプリケーションのメインエントリポイント
- `FormHiMenu.cs` - メインフォーム
- `CMenuPage.cs` - メニュー設定管理クラス
- `CMenuChain.cs` - メニュー階層管理クラス
- `CFunctions.cs` - ユーティリティ関数
- `FormAbout.cs` - バージョン情報画面
- `FormButtonEdit.cs` - ボタン編集画面
- `FormColor.cs` - 色選択画面
- `FormConfiguration.cs` - 環境設定画面
- `FormPassword.cs` - パスワード設定・確認画面
- `Extensions.cs` - VB.NET関数をC#で再現する拡張メソッド
- `SampleMenu.xml` - サンプルメニュー定義ファイル

## 使用方法

1. プロジェクトをビルド
2. 初回起動時は編集モードになっているので、ボタンをクリックして編集
3. 「ファイル」→「編集モード」→「実行モード」で実際に使用可能

### XML形式メニューファイルの使用

このバージョンからXML形式でのメニュー定義がサポートされています：

1. 「ファイル」→「新規作成」から新しいXMLメニューファイルを作成
2. 「ファイル」→「開く」から既存のXMLメニューファイルを開く
3. `SampleMenu.xml`にサンプルメニュー定義が含まれています

## XMLメニュー定義の構造

```xml
<?xml version="1.0" encoding="utf-8"?>
<HiMenu version="1.0.0.0">
  <EnvironmentTitle>
    <Title>メニュータイトル</Title>
    <Items>
      <Item index="1">
        <Title>ボタンのタイトル</Title>
        <Comment>ステータスバーに表示される説明</Comment>
        <Command>実行するコマンド</Command>
        <Flag>フラグ設定</Flag>
      </Item>
      <!-- 他のアイテム -->
    </Items>
  </EnvironmentTitle>
  <ExecuteEnvironment>
    <!-- 表示設定など -->
  </ExecuteEnvironment>
  <Current>
    <!-- 位置情報 -->
  </Current>
</HiMenu>
```

## トラブルシューティング

### 「リソースファイル(.resx)が見つかりません」エラー

プロジェクトをクローンした後に以下のエラーが発生する場合があります：

```
リソース ファイル "FormColor.resx" が見つかりません。
リソース ファイル "FormPassword.resx" が見つかりません。
リソース ファイル "FormHiMenu.resx" が見つかりません。
リソース ファイル "FormConfiguration.resx" が見つかりません。
リソース ファイル "FormButtonEdit.resx" が見つかりません。
リソース ファイル "FormAbout.resx" が見つかりません。
```

**解決策**:
このリポジトリには必要な.resxファイルが含まれています。しかし、もし上記のエラーが発生した場合は、Visual Studioで各フォームをデザインビューで開いて保存することで自動的に.resxファイルが生成されます。

あるいは、空の.resxファイルを手動で作成して対応することもできます。それぞれのフォームに対応する.resxファイルを作成し、基本的なXML構造を含める必要があります。

## 注意事項

- このバージョンではXML形式のメニュー定義ファイル（*.xml）を使用しています。
- このプロジェクトは.NET Framework 4.8をターゲットにしていますが、必要に応じて.NET 6以上に移行することも検討できます。
- Git経由でプロジェクトを共有する際は、.gitignoreが適切に設定されていることを確認してください。

