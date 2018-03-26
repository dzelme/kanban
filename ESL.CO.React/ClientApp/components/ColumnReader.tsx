import * as React from 'react';
import BoardName from './BoardName';
import BoardTable from './BoardTable';
import { Value, Board, Credentials } from './Interfaces';
import { ApiClient } from './ApiClient';

interface ColumnReaderState {
    boardlist: Value[];
    currentIndex: number;
    boardId: number;
    board: Board;
    boardChanged: boolean;
    loading: boolean;
    titleList: string[];
}

// test when no appSettings.json - currently creates error @boardId: this.props.boardlist[0].id
// error because generated file hass all boards with visibility false
export default class ColumnReader extends React.Component<{ boardlist: Value[], credentials: Credentials, titleList: string[] }, ColumnReaderState> {
    refreshTimer: number;
    
    constructor(props) {
        super(props);
        this.state = {
            boardlist: this.props.boardlist,
            currentIndex: 0,
            boardId: this.props.boardlist[0].id,
            board: {
                id: 0, name: "", fromCache: false, message: "", columns: [], rows: [], hasChanged: false
            },
            boardChanged: false,
            loading: true,
            titleList: this.props.titleList
        };

        this.nextSlide = this.nextSlide.bind(this);


        ApiClient.boardData(this.state.boardId, this.props.credentials)
            .then(data => {
                if (this.state.titleList.length == 0) {
                    this.setState({ board: data, boardChanged: true }, this.makeTitleList);
                }
                else {
                    this.setState({ board: data, loading: false, boardChanged: true }, this.RefreshRate);
                }
            });
    }

    makeTitleList() {
        let list = [];

        list.push(this.state.board.name);
        
        this.setState({ titleList: list, loading: false }, this.RefreshRate);
    }

    nextSlide() {

        var index;

        if (this.state.currentIndex == (this.state.boardlist.length - 1)) {
            index = 0;
        }
        else {
            index = this.state.currentIndex + 1;
        }


        const newState = {
            currentIndex: index,
            boardId: this.state.boardlist[index].id,
            boardChanged: false
        }

        this.setState(newState, this.boardLoad);
    }


    slideShow() {
        this.increment();  //AD: increments timesShown board statistic
        setTimeout(this.nextSlide, this.state.boardlist[this.state.currentIndex].timeShown);
    }

    boardLoad() {
        clearInterval(this.refreshTimer);
        
        ApiClient.boardData(this.state.boardId, this.props.credentials)
            .then(data => {
                if (data.id == this.state.boardId) {
                    if (this.state.board.id == data.id && data.hasChanged == false) {
                        this.setState({ board: data, boardChanged: false }, this.RefreshRate);
                    }
                    else {
                        this.setState({ board: data, boardChanged: true }, this.RefreshRate);
                    }
                }
            });
    }

    RefreshRate() {
        this.refreshTimer = setInterval(
            () => this.boardLoad(),
            this.state.boardlist[this.state.currentIndex].refreshRate
        );
    }

    //AD: increments timesShown board statistic
    increment() {
        ApiClient.incrementTimesShown(this.state.boardId);
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

                    <div>  <BoardName name={this.state.board.name} fromCache={this.state.board.fromCache} message={this.state.board.message} allNames={this.state.titleList} /></div>
                    <div id='board'><BoardTable board={this.state.board} /></div>

                    { (this.state.boardlist.length <= 1) ? this.increment() : this.slideShow() }

                </div>;
            }
        }
    }
}