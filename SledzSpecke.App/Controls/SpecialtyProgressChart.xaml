<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:charts="clr-namespace:SledzSpecke.App.Controls.Charts"
             x:Class="SledzSpecke.App.Controls.SpecialtyProgressChart">

    <ContentView.Resources>
        <ResourceDictionary>
            <!-- Style dla obszarów wykresów -->
            <Style x:Key="ChartAreaStyle" TargetType="Border">
                <Setter Property="Padding" Value="10" />
                <Setter Property="BackgroundColor" Value="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}" />
                <Setter Property="Stroke" Value="{AppThemeBinding Light={StaticResource Gray300}, Dark={StaticResource Gray700}}" />
                <Setter Property="StrokeThickness" Value="1" />
                <Setter Property="StrokeShape" Value="RoundRectangle 10" />
                <Setter Property="Margin" Value="0,0,0,15" />
            </Style>
            
            <!-- Style dla tytułów wykresów -->
            <Style x:Key="ChartTitleStyle" TargetType="Label">
                <Setter Property="FontSize" Value="16" />
                <Setter Property="FontAttributes" Value="Bold" />
                <Setter Property="HorizontalOptions" Value="Center" />
                <Setter Property="Margin" Value="0,0,0,10" />
            </Style>
            
            <!-- Style dla etykiet osi -->
            <Style x:Key="AxisLabelStyle" TargetType="Label">
                <Setter Property="FontSize" Value="12" />
                <Setter Property="TextColor" Value="{AppThemeBinding Light={StaticResource Gray700}, Dark={StaticResource Gray300}}" />
            </Style>
        </ResourceDictionary>
    </ContentView.Resources>

    <Grid RowDefinitions="Auto,Auto,Auto,Auto" Padding="10">
        <!-- Nagłówek -->
        <Label Text="Postęp specjalizacji" Style="{StaticResource HeaderLabel}" Grid.Row="0" />
        
        <!-- Wykres kołowy postępu ogólnego -->
        <Border Grid.Row="1" Style="{StaticResource ChartAreaStyle}">
            <Grid RowDefinitions="Auto,*" RowSpacing="10">
                <Label Text="Ogólny postęp" Style="{StaticResource ChartTitleStyle}" />
                
                <Grid Grid.Row="1" ColumnDefinitions="*,*">
                    <!-- Wykres kołowy -->
                    <charts:CircularProgressChart 
                        Progress="{Binding OverallProgress}" 
                        ProgressColor="{StaticResource Primary}"
                        TrackColor="{AppThemeBinding Light={StaticResource Gray200}, Dark={StaticResource Gray800}}"
                        StrokeWidth="15"
                        HeightRequest="150"
                        WidthRequest="150" />
                    
                    <!-- Legenda -->
                    <VerticalStackLayout Grid.Column="1" Spacing="10" VerticalOptions="Center">
                        <Label Text="{Binding OverallProgressText, StringFormat='Postęp: {0}'}" 
                               FontSize="20" 
                               FontAttributes="Bold" 
                               TextColor="{StaticResource Primary}" />
                        
                        <Label Text="{Binding TimeLeftText}" 
                               FontSize="14" />
                               
                        <Button Text="Szczegóły" 
                                Command="{Binding ViewDetailsCommand}"
                                Style="{StaticResource PrimaryButton}"
                                Margin="0,10,0,0" />
                    </VerticalStackLayout>
                </Grid>
            </Grid>
        </Border>
        
        <!-- Wykres słupkowy dla kategorii -->
        <Border Grid.Row="2" Style="{StaticResource ChartAreaStyle}">
            <Grid RowDefinitions="Auto,*" RowSpacing="10">
                <Label Text="Postęp według kategorii" Style="{StaticResource ChartTitleStyle}" />
                
                <charts:BarChart 
                    Grid.Row="1"
                    ItemsSource="{Binding CategoryProgress}"
                    ValuePath="Value"
                    CategoryPath="Key"
                    BarColor="{StaticResource Primary}"
                    HeightRequest="200" />
            </Grid>
        </Border>
        
        <!-- Wykres liniowy postępu w czasie -->
        <Border Grid.Row="3" Style="{StaticResource ChartAreaStyle}">
            <Grid RowDefinitions="Auto,*" RowSpacing="10">
                <Label Text="Postęp w czasie" Style="{StaticResource ChartTitleStyle}" />
                
                <charts:LineChart 
                    Grid.Row="1"
                    ItemsSource="{Binding ProgressOverTime}"
                    XValuePath="Key"
                    YValuePath="Value"
                    LineColor="{StaticResource Accent}"
                    PointColor="{StaticResource Primary}"
                    HeightRequest="200" />
            </Grid>
        </Border>
    </Grid>
</ContentView>
