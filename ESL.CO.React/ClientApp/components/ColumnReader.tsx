import * as React from 'react';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { ColumnReaderState, Value } from './Interfaces';
import { ApiClient } from './ApiClient';

// test when no appSettings.json - currently creates error @boardId: this.props.boardlist[0].id
// error because generated file hass all boards with visibility false
export default class ColumnReader extends React.Component<{ boardList: Value[], presentationID: string, titleList: string[] }, ColumnReaderState> {
    refreshTimer: number;
    
    constructor(props) {
        super(props);
        this.state = {
            boardList: this.props.boardList,
            currentIndex: 0,
            boardId: this.props.boardList[0].id,
            board: {
                id: 0, name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            loading: true,
        };

        this.nextSlide = this.nextSlide.bind(this);

        ApiClient.getAPresentation(this.props.presentationID)
            .then(dataPres => {

                ApiClient.boardData(this.state.boardId, dataPres.credentials)
                    .then(dataBoard => {
                          this.setState({ board: dataBoard, loading: false, boardChanged: true }, this.RefreshRate);
                    });
            });     
    }

    nextSlide() {

        var index;

        if (this.state.currentIndex == (this.state.boardList.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }


        const newState = {
            currentIndex: index,
            boardId: this.state.boardList[index].id,
            boardChanged: false
        }

        this.setState(newState, this.boardLoad);
    }


    slideShow() {
        this.increment();  //AD: increments timesShown board statistic
        setTimeout(this.nextSlide, this.state.boardList[this.state.currentIndex].timeShown*1000);
    }

    boardLoad() {
        clearInterval(this.refreshTimer);

        ApiClient.getAPresentation(this.props.presentationID)
            .then(dataPres => {

                ApiClient.boardData(this.state.boardId, dataPres.credentials)
                    .then(dataBoard => {
                        if (dataBoard.id == this.state.boardId) {
                            if (this.state.board.id == dataBoard.id && dataBoard.hasChanged == false) {
                                this.setState({ board: dataBoard, boardChanged: false }, this.RefreshRate);
                            }
                            else {
                                this.setState({ board: dataBoard, boardChanged: true }, this.RefreshRate);
                            }
                        }
                    });
            });      
    }

    RefreshRate() {
        this.refreshTimer = setInterval(
            () => this.boardLoad(),
            this.state.boardList[this.state.currentIndex].refreshRate
        );
    }

    //AD: increments timesShown board statistic
    increment() {
        ApiClient.saveToStatistics(this.state.board.id.toString(), this.state.board.name);
    }

    shouldComponentUpdate(nextProps, nextState) {
        return nextState.boardChanged;
    }

    componentWillUnmount() {
        clearInterval(this.refreshTimer);
    }

    public render() {

        if (this.state.loading) {
            return <h1>Loading...</h1>
        }
        else {

            if (this.state.board.columns.length == 0) {
                return <h1>Error loading!</h1>
            }
            else {
                return <div>

                    <div>  <BoardName presentationId={this.props.presentationID} name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} boardlist={this.state.boardList} /></div>
                    <div id='board'><BoardTable board={this.state.board} /></div>

                    { (this.state.boardList.length <= 1) ? this.increment() : this.slideShow() }

                </div>;
            }
        }
    }
}