# .NET 10 private 메서드 테스트 비교

동일한 private 메서드를 Reflection, UnsafeAccessor, Source Generator의 세 가지 방식으로 테스트하는 샘플 솔루션입니다.

## 프로젝트 구성

| 프로젝트 | 대상 프레임워크 | 역할 |
| --- | --- | --- |
| Net10Sample | net10.0 | 할인 계산 클래스 및 TestExpose 특성 |
| Net10Sample.Tests | net10.0 | xUnit 테스트와 케이스 데이터 |
| Net10Sample.TestGenerator | netstandard2.0 | 테스트용 호출 래퍼를 생성하는 Roslyn incremental source generator |

`Net10Sample.sln`을 Visual Studio에서 열거나 .NET CLI로 빌드할 수 있습니다. .NET 10 SDK 및 .NET 10을 지원하는 Visual Studio가 필요합니다. 최초 복원에는 NuGet 패키지가 필요합니다.

## 테스트 대상

`PriceCalculator.CalculateDiscountAmount(decimal price, decimal discountPercent)`는 가격과 할인율로 할인 금액을 계산하는 private 메서드입니다.

- 가격은 0 이상, 할인율은 0~100이어야 합니다.
- 잘못된 값에는 ArgumentOutOfRangeException을 발생시킵니다.
- 할인 금액은 소수점 둘째 자리까지 MidpointRounding.AwayFromZero로 반올림합니다.
- public CalculateFinalPrice는 가격에서 할인 금액을 뺀 결과를 반환합니다.

## 세 가지 테스트 방식

| 테스트 클래스 | 호출 방식 | 예외 검증 |
| --- | --- | --- |
| PriceCalculatorReflectionTests | GetMethod 및 MethodInfo.Invoke | TargetInvocationException.InnerException 확인 |
| PriceCalculatorUnsafeAccessorTests | UnsafeAccessor 특성이 붙은 static extern 접근자 | ArgumentOutOfRangeException 직접 확인 |
| PriceCalculatorSourceGeneratorTests | 생성된 Test_CalculateDiscountAmount 래퍼 호출 | ArgumentOutOfRangeException 직접 확인 |

각 방식은 정상 계산, 가격 0, 할인율 0 및 100, 소수 할인율, 반올림 경계, 잘못된 입력 등 동일한 11개 케이스를 검증합니다. 각 클래스는 partial 선언으로 분리되며 TheoryData는 별도의 .TestData.cs 파일에 있습니다. 프로젝트의 DependentUpon 설정으로 Visual Studio에서 데이터 파일을 해당 테스트 파일 아래에 묶습니다.

## Source Generator 동작

1. 원본 private 메서드에 [TestExpose]를 붙입니다.
2. 생성기가 해당 메서드를 찾아 같은 타입의 partial 클래스 코드를 생성합니다.
3. 생성된 public Test_메서드는 원본 private 메서드를 직접 호출합니다.
4. 원본 클래스도 partial이어야 합니다. 생성기는 원본 프로젝트에 Analyzer로 참조됩니다.

Debug 및 Test 구성에서만 원본과 테스트 프로젝트에 TEST_EXPOSE 상수가 정의됩니다. 생성된 래퍼와 해당 테스트는 이 상수가 있을 때만 컴파일됩니다. Release에는 테스트용 public 래퍼가 포함되지 않습니다.

생성기 자체는 Visual Studio 및 CLI 컴파일러 호스트와의 호환성을 위해 netstandard2.0을 대상으로 합니다. 이 생성기는 현재 PriceCalculator와 같은 일반 클래스의 비제네릭 인스턴스 메서드를 위한 샘플이며, 중첩 타입, 제네릭, static 및 ref/out 호출 등으로 확장하려면 생성 규칙을 추가해야 합니다.

## 빌드 및 실행

저장소 루트에서 실행합니다.

```powershell
dotnet restore Net10Sample.sln
dotnet build Net10Sample.sln
dotnet run --project Net10Sample
```

기본 콘솔 프로그램은 Hello, World!를 출력합니다.

## 테스트 실행

```powershell
# Debug: 세 방식의 전체 33개 테스트
dotnet test Net10Sample.sln

# Test: 세 방식의 전체 33개 테스트
dotnet test Net10Sample.sln -c Test

# Release: Reflection 및 UnsafeAccessor의 22개 테스트
dotnet test Net10Sample.sln -c Release
```

Visual Studio에서는 Debug 또는 Test 구성으로 빌드한 다음 테스트 탐색기에서 전체 테스트를 실행합니다. Release에서는 Source Generator 테스트 선언이 조건부 컴파일로 제외됩니다.

## 주의사항

private 메서드 직접 테스트는 내부 구현에 의존하므로 이름이나 시그니처를 변경하면 테스트도 수정해야 합니다. UnsafeAccessor의 대상 연결은 런타임에 검사됩니다. 생성기 래퍼 테스트는 public CalculateFinalPrice의 전체 동작을 대신 검증하지 않습니다.

bin, obj, .vs, 테스트 결과 및 로컬 SDK 캐시는 Git 추적에서 제외합니다.
